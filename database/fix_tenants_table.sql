-- ============================================
-- FIX EXISTING TENANTS TABLE
-- ============================================
-- Run this if you already have a tenants table without the domain column

-- 1. Check current structure
SELECT 
  column_name,
  data_type,
  is_nullable
FROM information_schema.columns
WHERE table_name = 'tenants'
ORDER BY ordinal_position;

-- 2. Add missing columns
ALTER TABLE tenants ADD COLUMN IF NOT EXISTS domain TEXT;
ALTER TABLE tenants ADD COLUMN IF NOT EXISTS description TEXT;
ALTER TABLE tenants ADD COLUMN IF NOT EXISTS updated_at TIMESTAMPTZ DEFAULT NOW();

-- 3. Add unique constraint to domain
ALTER TABLE tenants DROP CONSTRAINT IF EXISTS tenants_domain_key;
ALTER TABLE tenants ADD CONSTRAINT tenants_domain_key UNIQUE (domain);

-- 4. Create index on domain
CREATE INDEX IF NOT EXISTS idx_tenants_domain ON tenants(domain);

-- 5. Verify changes
SELECT 
  column_name,
  data_type,
  is_nullable,
  column_default
FROM information_schema.columns
WHERE table_name = 'tenants'
ORDER BY ordinal_position;

-- 6. Show current tenants
SELECT * FROM tenants;
