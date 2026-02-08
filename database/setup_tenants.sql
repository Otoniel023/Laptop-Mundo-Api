-- ============================================
-- TENANT TABLE MIGRATION - SAFE VERSION
-- ============================================
-- This script safely creates or updates the tenants table

-- 1. CREATE TENANTS TABLE (only if it doesn't exist)
CREATE TABLE IF NOT EXISTS tenants (
  id BIGSERIAL PRIMARY KEY,
  name TEXT NOT NULL,
  created_at TIMESTAMPTZ DEFAULT NOW()
);

-- 2. ADD MISSING COLUMNS (safe - won't fail if they exist)
DO $$ 
BEGIN
  -- Add domain column if it doesn't exist
  IF NOT EXISTS (
    SELECT 1 FROM information_schema.columns 
    WHERE table_name = 'tenants' AND column_name = 'domain'
  ) THEN
    ALTER TABLE tenants ADD COLUMN domain TEXT;
  END IF;

  -- Add description column if it doesn't exist
  IF NOT EXISTS (
    SELECT 1 FROM information_schema.columns 
    WHERE table_name = 'tenants' AND column_name = 'description'
  ) THEN
    ALTER TABLE tenants ADD COLUMN description TEXT;
  END IF;

  -- Add updated_at column if it doesn't exist
  IF NOT EXISTS (
    SELECT 1 FROM information_schema.columns 
    WHERE table_name = 'tenants' AND column_name = 'updated_at'
  ) THEN
    ALTER TABLE tenants ADD COLUMN updated_at TIMESTAMPTZ DEFAULT NOW();
  END IF;
END $$;

-- 3. ADD UNIQUE CONSTRAINT TO DOMAIN (if not exists)
DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM pg_constraint 
    WHERE conname = 'tenants_domain_key'
  ) THEN
    ALTER TABLE tenants ADD CONSTRAINT tenants_domain_key UNIQUE (domain);
  END IF;
END $$;

-- 4. CREATE OR REPLACE TRIGGER FUNCTION FOR UPDATED_AT
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $func$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$func$ LANGUAGE plpgsql;

-- 5. CREATE TRIGGER (drop if exists first)
DROP TRIGGER IF EXISTS update_tenants_updated_at ON tenants;
CREATE TRIGGER update_tenants_updated_at 
  BEFORE UPDATE ON tenants
  FOR EACH ROW 
  EXECUTE FUNCTION update_updated_at_column();

-- 6. ENABLE ROW LEVEL SECURITY
ALTER TABLE tenants ENABLE ROW LEVEL SECURITY;

-- 7. DROP EXISTING POLICIES (to avoid conflicts)
DROP POLICY IF EXISTS "Public tenants are viewable by everyone" ON tenants;
DROP POLICY IF EXISTS "Super admins can insert tenants" ON tenants;
DROP POLICY IF EXISTS "Super admins can update tenants" ON tenants;
DROP POLICY IF EXISTS "Super admins can delete tenants" ON tenants;

-- 8. CREATE POLICIES FOR TENANTS TABLE

-- Policy: Anyone can read tenants (for listing available stores)
CREATE POLICY "Public tenants are viewable by everyone"
ON tenants FOR SELECT
USING (true);

-- Policy: Authenticated users can create tenants (for now)
-- TODO: Later restrict to super admins only
CREATE POLICY "Super admins can insert tenants"
ON tenants FOR INSERT
WITH CHECK (auth.role() = 'authenticated');

-- Policy: Authenticated users can update tenants
CREATE POLICY "Super admins can update tenants"
ON tenants FOR UPDATE
USING (auth.role() = 'authenticated');

-- Policy: Authenticated users can delete tenants
CREATE POLICY "Super admins can delete tenants"
ON tenants FOR DELETE
USING (auth.role() = 'authenticated');

-- 9. CREATE HELPER FUNCTION TO GET CURRENT USER'S TENANT_ID
CREATE OR REPLACE FUNCTION auth.current_tenant_id()
RETURNS BIGINT AS $$
  SELECT COALESCE(
    (auth.jwt() -> 'user_metadata' ->> 'tenant_id')::BIGINT,
    0
  );
$$ LANGUAGE sql STABLE;

-- 10. CREATE INDEXES FOR PERFORMANCE (if not exist)
CREATE INDEX IF NOT EXISTS idx_tenants_domain ON tenants(domain);
CREATE INDEX IF NOT EXISTS idx_tenant_products_tenant_id ON tenant_products(tenant_id);
CREATE INDEX IF NOT EXISTS idx_discounts_tenant_id ON discounts(tenant_id);

-- 11. ADD COMMENTS
COMMENT ON TABLE tenants IS 'Stores (tenants) in the multi-tenant system. Each tenant has its own products, pricing, and inventory.';
COMMENT ON COLUMN tenants.id IS 'Unique identifier for the tenant';
COMMENT ON COLUMN tenants.name IS 'Display name of the tenant/store';
COMMENT ON COLUMN tenants.domain IS 'Unique domain for the tenant (e.g., store1.laptopmundo.com)';
COMMENT ON COLUMN tenants.description IS 'Description of the tenant/store';

-- 12. INSERT SAMPLE TENANTS (only if they don't exist)
INSERT INTO tenants (name, domain, description) 
SELECT 'LaptopMundo Store 1', 'store1.laptopmundo.com', 'Primera tienda de laptops'
WHERE NOT EXISTS (
  SELECT 1 FROM tenants WHERE domain = 'store1.laptopmundo.com'
);

INSERT INTO tenants (name, domain, description) 
SELECT 'LaptopMundo Store 2', 'store2.laptopmundo.com', 'Segunda tienda de laptops'
WHERE NOT EXISTS (
  SELECT 1 FROM tenants WHERE domain = 'store2.laptopmundo.com'
);

-- 13. VERIFY SETUP
SELECT 
  'Tenants Table Setup' as status,
  COUNT(*) as tenant_count,
  'SUCCESS' as result
FROM tenants;

-- Show table structure
SELECT 
  column_name,
  data_type,
  is_nullable,
  column_default
FROM information_schema.columns
WHERE table_name = 'tenants'
ORDER BY ordinal_position;
