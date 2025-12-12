# InventorySystem - Windows Forms (C#, EF Core, MySQL) - CORREGIDO

**Cambios principales en esta versión (corrección solicitada)**:
- Puerto de MySQL configurado en `AppSettings.json` a **3308**.
- `Product.SupplierId` ahora es nullable (`int?`) y la tabla `Products` permite `SupplierId NULL` para que la eliminación de un proveedor ponga `NULL` en productos (coincide con `ON DELETE SET NULL`).
- Se reemplazó la llamada a `Database.Migrate()` por `Database.EnsureCreated()` para evitar errores si no se ejecutan migraciones en el entorno del revisor.
- Se añadieron validaciones mínimas y manejo de casos donde no existan proveedores.

Sigue las mismas instrucciones de ejecución del README original incluido en el ZIP anterior.
