# 🏢 Multi-Tenancy - Guía Completa

## 📋 Concepto

El sistema usa **multi-tenancy** para permitir que múltiples tiendas (tenants) compartan la misma infraestructura pero mantengan sus datos separados.

## 🔐 Flujo de Autenticación

### 1. Login y JWT

```typescript
// Usuario hace login
POST /api/auth/login
{
  "email": "admin@tienda1.com",
  "password": "password123"
}

// Supabase retorna JWT con claims:
{
  "sub": "user-uuid-123",
  "email": "admin@tienda1.com",
  "tenant_id": 1,  // ← CLAVE PARA MULTI-TENANCY
  "role": "admin",
  "exp": 1234567890
}
```

### 2. Almacenamiento en Frontend

```typescript
// apiClient.ts guarda el token
localStorage.setItem('access_token', jwt_token);

// Cada request incluye el token
headers: {
  'Authorization': `Bearer ${token}`
}
```

### 3. Extracción en Backend

```csharp
// AdminProductEndpoints.cs - Línea 217-225
private static long GetTenantId(HttpContext context)
{
    // Extrae el claim "tenant_id" del JWT
    var tenantIdClaim = context.User.FindFirst("tenant_id")?.Value;

    if (long.TryParse(tenantIdClaim, out var tenantId))
    {
        return tenantId;
    }

    throw new UnauthorizedAccessException("Tenant ID not found in token");
}
```

## 🗄️ Estructura de Base de Datos

### Tablas Principales

```sql
-- 1. TENANTS (Tiendas)
CREATE TABLE tenants (
  id BIGSERIAL PRIMARY KEY,
  name TEXT NOT NULL,              -- "LaptopMundo Store 1"
  domain TEXT UNIQUE,              -- "tienda1.laptopmundo.com"
  created_at TIMESTAMPTZ DEFAULT NOW()
);

-- 2. USERS (Usuarios por Tenant)
CREATE TABLE users (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  tenant_id BIGINT REFERENCES tenants(id),  -- ← Asociación al tenant
  email TEXT UNIQUE NOT NULL,
  role TEXT DEFAULT 'customer',    -- 'admin' o 'customer'
  created_at TIMESTAMPTZ DEFAULT NOW()
);

-- 3. PRODUCTS (Productos Base - Compartidos)
CREATE TABLE products (
  id BIGSERIAL PRIMARY KEY,
  name TEXT NOT NULL,              -- "Dell XPS 15"
  description TEXT,
  category_id BIGINT REFERENCES categories(id)
  -- NO tiene tenant_id porque es compartido
);

-- 4. TENANT_PRODUCTS (Productos por Tenant)
CREATE TABLE tenant_products (
  id BIGSERIAL PRIMARY KEY,
  tenant_id BIGINT REFERENCES tenants(id),    -- ← Filtro por tenant
  product_id BIGINT REFERENCES products(id),
  price DECIMAL(10,2) NOT NULL,              -- Precio específico del tenant
  inventory_count INTEGER DEFAULT 0,          -- Inventario específico
  is_visible BOOLEAN DEFAULT true,
  UNIQUE(tenant_id, product_id)
);

-- 5. DISCOUNTS (Descuentos por Tenant)
CREATE TABLE discounts (
  id BIGSERIAL PRIMARY KEY,
  tenant_id BIGINT REFERENCES tenants(id),    -- ← Filtro por tenant
  name TEXT NOT NULL,
  discount_type TEXT NOT NULL,
  value DECIMAL(10,2) NOT NULL,
  start_date TIMESTAMPTZ NOT NULL,
  end_date TIMESTAMPTZ NOT NULL,
  is_active BOOLEAN DEFAULT true
);
```

## 🔄 Flujo Completo de una Operación

### Ejemplo: Listar Productos

```typescript
// 1. FRONTEND - Usuario autenticado hace request
const products = await apiClient.get('/api/products');

// 2. apiClient.ts agrega el token
headers: {
  'Authorization': 'Bearer eyJhbGc...' // JWT con tenant_id=1
}

// 3. BACKEND - ProductEndpoints.cs recibe request
app.MapGet("/api/products", async (
    HttpContext context,
    IProductService service) =>
{
    // Extrae tenant_id del JWT
    var tenantId = GetTenantId(context);  // tenantId = 1

    // Pasa tenant_id al servicio
    var result = await service.GetProductsAsync(tenantId, filter);
    return Results.Ok(result);
});

// 4. BACKEND - ProductService.cs filtra por tenant
public async Task<PaginatedResponseDto<ProductDto>> GetProductsAsync(
    long tenantId,
    ProductFilterDto filter)
{
    // Query SIEMPRE filtra por tenant_id
    var query = _supabaseClient
        .From<TenantProductModel>()
        .Filter("tenant_id", Constants.Operator.Equals, tenantId)  // ← FILTRO
        .Filter("is_visible", Constants.Operator.Equals, true);

    // ... resto de la lógica
}

// 5. RESULTADO - Solo productos del tenant 1
[
  { id: 1, name: "Dell XPS 15", price: 1299.99, tenantId: 1 },
  { id: 2, name: "HP Spectre", price: 1499.99, tenantId: 1 }
]
// NO incluye productos de tenant 2, 3, etc.
```

## 🎯 Casos de Uso

### Caso 1: Dos Tiendas, Mismo Producto, Precios Diferentes

```sql
-- Producto base (compartido)
INSERT INTO products (id, name, description)
VALUES (1, 'Dell XPS 15', 'Laptop profesional');

-- Tenant 1: Vende a $1,299
INSERT INTO tenant_products (tenant_id, product_id, price, inventory_count)
VALUES (1, 1, 1299.99, 10);

-- Tenant 2: Vende a $1,499
INSERT INTO tenant_products (tenant_id, product_id, price, inventory_count)
VALUES (2, 1, 1499.99, 5);
```

**Resultado:**

- Usuario de Tenant 1 ve: Dell XPS 15 - $1,299.99
- Usuario de Tenant 2 ve: Dell XPS 15 - $1,499.99

### Caso 2: Descuentos Exclusivos por Tenant

```sql
-- Tenant 1: Black Friday 20%
INSERT INTO discounts (tenant_id, name, discount_type, value)
VALUES (1, 'Black Friday', 'percentage', 20);

-- Tenant 2: Cyber Monday 15%
INSERT INTO discounts (tenant_id, name, discount_type, value)
VALUES (2, 'Cyber Monday', 'percentage', 15);
```

## 🛡️ Seguridad

### Row Level Security (RLS) en Supabase

```sql
-- Habilitar RLS en tenant_products
ALTER TABLE tenant_products ENABLE ROW LEVEL SECURITY;

-- Policy: Solo ver productos de tu tenant
CREATE POLICY "Users can only see their tenant's products"
ON tenant_products
FOR SELECT
USING (tenant_id = (auth.jwt() ->> 'tenant_id')::bigint);

-- Policy: Solo admins pueden modificar
CREATE POLICY "Only admins can modify products"
ON tenant_products
FOR ALL
USING (
  tenant_id = (auth.jwt() ->> 'tenant_id')::bigint
  AND (auth.jwt() ->> 'role') = 'admin'
);
```

## 📝 Configuración en Supabase

### 1. Crear Función para Extraer Tenant ID

```sql
-- Función helper para obtener tenant_id del JWT
CREATE OR REPLACE FUNCTION auth.tenant_id()
RETURNS bigint AS $$
  SELECT COALESCE(
    (auth.jwt() ->> 'tenant_id')::bigint,
    0
  );
$$ LANGUAGE sql STABLE;
```

### 2. Configurar JWT en Supabase

En el Dashboard de Supabase → Authentication → Settings:

```json
{
  "jwt_secret": "your-super-secret-key",
  "jwt_exp": 3600,
  "additional_claims": {
    "tenant_id": "user_metadata.tenant_id",
    "role": "user_metadata.role"
  }
}
```

### 3. Crear Usuario con Tenant

```typescript
// Cuando creas un usuario en Supabase
const { data, error } = await supabase.auth.signUp({
  email: "admin@tienda1.com",
  password: "password123",
  options: {
    data: {
      tenant_id: 1, // ← Asignar tenant
      role: "admin", // ← Asignar rol
    },
  },
});
```

## 🔧 Implementación en el Frontend

### Componente para Asignar Producto a Tenant

```typescript
// CreateTenantProductForm.tsx
import { useState } from 'react';
import { adminProductsApi } from '../../api/admin.products.api';

export function CreateTenantProductForm() {
  const [formData, setFormData] = useState({
    productId: 0,
    price: 0,
    inventoryCount: 0,
    isVisible: true,
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // El tenant_id se extrae automáticamente del JWT en el backend
    await adminProductsApi.createTenantProduct(formData);

    alert('Producto agregado a tu tienda!');
  };

  return (
    <form onSubmit={handleSubmit}>
      <input
        type="number"
        placeholder="ID del Producto"
        value={formData.productId}
        onChange={(e) => setFormData({
          ...formData,
          productId: Number(e.target.value)
        })}
      />
      <input
        type="number"
        placeholder="Precio"
        value={formData.price}
        onChange={(e) => setFormData({
          ...formData,
          price: Number(e.target.value)
        })}
      />
      <input
        type="number"
        placeholder="Inventario"
        value={formData.inventoryCount}
        onChange={(e) => setFormData({
          ...formData,
          inventoryCount: Number(e.target.value)
        })}
      />
      <button type="submit">Agregar a Mi Tienda</button>
    </form>
  );
}
```

## 🚀 Ventajas del Sistema Multi-Tenant

1. **Aislamiento de Datos**: Cada tienda solo ve sus propios datos
2. **Precios Flexibles**: Cada tenant puede tener precios diferentes
3. **Inventario Independiente**: Stock separado por tienda
4. **Descuentos Personalizados**: Promociones específicas por tenant
5. **Escalabilidad**: Una sola base de datos para múltiples tiendas
6. **Mantenimiento Simplificado**: Un solo código para todos

## ⚠️ Consideraciones Importantes

1. **SIEMPRE filtrar por tenant_id** en todas las queries
2. **Validar tenant_id** en el backend, nunca confiar en el frontend
3. **Usar RLS** en Supabase para seguridad adicional
4. **Auditar accesos** para detectar intentos de acceso cruzado
5. **Backup por tenant** para recuperación selectiva

## 🔍 Debugging

### Ver el Tenant ID del Usuario Actual

```typescript
// En el frontend
const token = localStorage.getItem("access_token");
const decoded = JSON.parse(atob(token.split(".")[1]));
console.log("Tenant ID:", decoded.tenant_id);
console.log("Role:", decoded.role);
```

### Verificar Filtros en Queries

```csharp
// En el backend, agregar logging
_logger.LogInformation($"Querying products for tenant: {tenantId}");
```

## 📚 Recursos Adicionales

- [Supabase Multi-Tenancy Guide](https://supabase.com/docs/guides/auth/row-level-security)
- [JWT Claims Documentation](https://jwt.io/introduction)
- [ASP.NET Core Authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/)
