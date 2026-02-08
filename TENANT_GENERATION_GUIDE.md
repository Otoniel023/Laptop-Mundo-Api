# 🏢 Guía Completa: Cómo Generar Tenants

## 📋 Tabla de Contenidos

1. [Configuración Inicial](#configuración-inicial)
2. [Crear Tenant desde la API](#crear-tenant-desde-la-api)
3. [Crear Usuario Admin para el Tenant](#crear-usuario-admin)
4. [Flujo Completo con Ejemplos](#flujo-completo)
5. [Verificación](#verificación)

---

## 🚀 Configuración Inicial

### Paso 1: Ejecutar el Script SQL en Supabase

1. Ve a tu proyecto en Supabase Dashboard
2. Navega a **SQL Editor**
3. Copia y pega el contenido de `database/setup_tenants.sql`
4. Click en **Run** para ejecutar

Esto creará:

- ✅ Tabla `tenants`
- ✅ Políticas RLS
- ✅ Triggers para `updated_at`
- ✅ Índices para performance
- ✅ 2 tenants de ejemplo

---

## 🏗️ Crear Tenant desde la API

### Opción 1: Usando cURL

```bash
# Crear un nuevo tenant
curl -X POST https://localhost:5001/api/tenants \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Mi Tienda de Laptops",
    "domain": "mitienda.laptopmundo.com",
    "description": "Tienda especializada en laptops gaming"
  }'

# Respuesta:
{
  "id": 3,
  "name": "Mi Tienda de Laptops",
  "domain": "mitienda.laptopmundo.com",
  "description": "Tienda especializada en laptops gaming",
  "createdAt": "2026-02-07T21:00:00Z"
}
```

### Opción 2: Usando Postman

1. **Method**: POST
2. **URL**: `https://localhost:5001/api/tenants`
3. **Headers**:
   ```
   Content-Type: application/json
   ```
4. **Body** (raw JSON):
   ```json
   {
     "name": "Mi Tienda de Laptops",
     "domain": "mitienda.laptopmundo.com",
     "description": "Tienda especializada en laptops gaming"
   }
   ```

### Opción 3: Usando el Frontend (React)

```typescript
// En tu componente de administración
const createTenant = async () => {
  const response = await fetch("https://localhost:5001/api/tenants", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      name: "Mi Tienda de Laptops",
      domain: "mitienda.laptopmundo.com",
      description: "Tienda especializada en laptops gaming",
    }),
  });

  const tenant = await response.json();
  console.log("Tenant creado:", tenant);
  return tenant;
};
```

---

## 👤 Crear Usuario Admin para el Tenant

### Paso 1: Obtener el ID del Tenant

Después de crear el tenant, guarda el `id` que retorna (ej: `3`)

### Paso 2: Crear el Usuario Admin

```bash
# Crear admin para el tenant con ID 3
curl -X POST https://localhost:5001/api/tenants/3/admin \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@mitienda.com",
    "password": "SecurePassword123!",
    "role": "admin"
  }'

# Respuesta:
{
  "id": "uuid-123-456",
  "tenantId": 3,
  "email": "admin@mitienda.com",
  "role": "admin",
  "createdAt": "2026-02-07T21:05:00Z"
}
```

### ¿Qué hace este endpoint?

1. **Crea el usuario en Supabase Auth**
2. **Asigna metadata al usuario**:
   ```json
   {
     "tenant_id": 3,
     "role": "admin"
   }
   ```
3. **Genera un JWT** que incluye estos claims
4. **El usuario ya puede hacer login**

---

## 🔄 Flujo Completo con Ejemplos

### Escenario: Crear una Nueva Tienda

```bash
# ===== PASO 1: CREAR TENANT =====
curl -X POST https://localhost:5001/api/tenants \
  -H "Content-Type: application/json" \
  -d '{
    "name": "TechStore Premium",
    "domain": "techstore.laptopmundo.com",
    "description": "Laptops de alta gama"
  }'

# Respuesta: { "id": 4, ... }

# ===== PASO 2: CREAR ADMIN DEL TENANT =====
curl -X POST https://localhost:5001/api/tenants/4/admin \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@techstore.com",
    "password": "Admin123!",
    "role": "admin"
  }'

# ===== PASO 3: LOGIN COMO ADMIN =====
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@techstore.com",
    "password": "Admin123!"
  }'

# Respuesta: { "accessToken": "eyJhbGc...", ... }

# ===== PASO 4: AGREGAR PRODUCTO AL TENANT =====
# El token JWT contiene tenant_id=4
curl -X POST https://localhost:5001/api/admin/products/tenant \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer eyJhbGc..." \
  -d '{
    "productId": 1,
    "price": 1299.99,
    "inventoryCount": 10,
    "isVisible": true
  }'

# El backend automáticamente usa tenant_id=4 del JWT
```

---

## 🧪 Verificación

### 1. Verificar que el Tenant Existe

```bash
# Listar todos los tenants
curl https://localhost:5001/api/tenants

# Obtener un tenant específico
curl https://localhost:5001/api/tenants/4
```

### 2. Verificar el Usuario en Supabase

1. Ve a **Supabase Dashboard** → **Authentication** → **Users**
2. Busca el email del admin
3. Click en el usuario
4. Verifica en **User Metadata**:
   ```json
   {
     "tenant_id": 4,
     "role": "admin"
   }
   ```

### 3. Verificar el JWT

```typescript
// En el frontend, después del login
const token = localStorage.getItem("access_token");
const payload = JSON.parse(atob(token.split(".")[1]));

console.log("Tenant ID:", payload.user_metadata.tenant_id); // 4
console.log("Role:", payload.user_metadata.role); // admin
```

### 4. Verificar Productos del Tenant

```bash
# Login como admin del tenant
# Luego listar productos (automáticamente filtrados por tenant_id)
curl https://localhost:5001/api/products \
  -H "Authorization: Bearer eyJhbGc..."

# Solo verás productos del tenant 4
```

---

## 📊 Diagrama de Flujo

```
┌─────────────────────────────────────────────────────────────┐
│                    GENERACIÓN DE TENANT                      │
└─────────────────────────────────────────────────────────────┘

1. POST /api/tenants
   ↓
   Crea registro en tabla `tenants`
   ↓
   Retorna: { id: 4, name: "TechStore", ... }

2. POST /api/tenants/4/admin
   ↓
   Llama a Supabase Auth SignUp
   ↓
   Crea usuario con metadata: { tenant_id: 4, role: "admin" }
   ↓
   Retorna: { id: "uuid", tenantId: 4, email: "...", ... }

3. POST /api/auth/login
   ↓
   Usuario hace login
   ↓
   Supabase genera JWT con claims:
   {
     sub: "uuid",
     email: "admin@techstore.com",
     user_metadata: {
       tenant_id: 4,
       role: "admin"
     }
   }
   ↓
   Frontend guarda token en localStorage

4. Requests subsecuentes
   ↓
   Frontend incluye: Authorization: Bearer <token>
   ↓
   Backend extrae tenant_id del JWT
   ↓
   Todas las queries filtran por tenant_id = 4
```

---

## 🔐 Seguridad

### Importante: Proteger Endpoints de Tenant

Los endpoints de creación de tenants deben estar protegidos:

```csharp
// En TenantEndpoints.cs
var group = app.MapGroup("/api/tenants")
    .WithTags("Tenants")
    .RequireAuthorization(); // ← Agregar esto

// Además, verificar que el usuario es super admin
private static bool IsSuperAdmin(HttpContext context)
{
    var role = context.User.FindFirst("role")?.Value;
    return role == "super_admin";
}
```

### Crear un Super Admin

```sql
-- En Supabase, crear el primer super admin manualmente
INSERT INTO auth.users (email, encrypted_password, email_confirmed_at, raw_user_meta_data)
VALUES (
  'superadmin@laptopmundo.com',
  crypt('SuperSecurePassword123!', gen_salt('bf')),
  NOW(),
  '{"role": "super_admin"}'::jsonb
);
```

---

## 🎯 Casos de Uso Comunes

### Caso 1: Onboarding de Nueva Tienda

```typescript
async function onboardNewStore(storeName: string, adminEmail: string) {
  // 1. Crear tenant
  const tenant = await createTenant({
    name: storeName,
    domain: `${storeName.toLowerCase().replace(/\s/g, "")}.laptopmundo.com`,
    description: `Tienda ${storeName}`,
  });

  // 2. Crear admin
  const admin = await createTenantAdmin(tenant.id, {
    email: adminEmail,
    password: generateSecurePassword(),
    role: "admin",
  });

  // 3. Enviar email de bienvenida
  await sendWelcomeEmail(adminEmail, admin.password);

  // 4. Crear categorías por defecto
  await createDefaultCategories(tenant.id);

  return { tenant, admin };
}
```

### Caso 2: Migrar Tienda Existente

```typescript
async function migrateTenant(oldTenantId: number, newDomain: string) {
  // 1. Crear nuevo tenant
  const newTenant = await createTenant({
    name: `Migrated from ${oldTenantId}`,
    domain: newDomain,
  });

  // 2. Copiar productos
  await copyProducts(oldTenantId, newTenant.id);

  // 3. Copiar usuarios
  await copyUsers(oldTenantId, newTenant.id);

  return newTenant;
}
```

---

## 📝 Checklist de Configuración

- [ ] Ejecutar `setup_tenants.sql` en Supabase
- [ ] Verificar que la tabla `tenants` existe
- [ ] Verificar políticas RLS
- [ ] Crear primer tenant de prueba
- [ ] Crear admin para el tenant
- [ ] Hacer login con el admin
- [ ] Verificar que el JWT contiene `tenant_id`
- [ ] Agregar un producto al tenant
- [ ] Verificar que solo se ven productos del tenant

---

## 🆘 Troubleshooting

### Error: "Tenant ID not found in token"

**Causa**: El JWT no contiene el claim `tenant_id`

**Solución**:

1. Verifica que el usuario fue creado con metadata
2. Haz logout y login nuevamente
3. Verifica el JWT decodificado

### Error: "Cannot insert into tenants table"

**Causa**: Políticas RLS bloqueando la inserción

**Solución**:

```sql
-- Temporalmente deshabilitar RLS para testing
ALTER TABLE tenants DISABLE ROW LEVEL SECURITY;

-- Después de crear tenants, rehabilitar
ALTER TABLE tenants ENABLE ROW LEVEL SECURITY;
```

### Error: "User already exists"

**Causa**: El email ya está registrado en Supabase Auth

**Solución**:

1. Usa un email diferente
2. O elimina el usuario existente desde Supabase Dashboard

---

## 📚 Recursos Adicionales

- [Supabase Auth Documentation](https://supabase.com/docs/guides/auth)
- [Row Level Security Guide](https://supabase.com/docs/guides/auth/row-level-security)
- [JWT Claims](https://jwt.io/introduction)
