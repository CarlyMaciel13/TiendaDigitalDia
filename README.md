**Funcionamiento**

*GESTION DE PRODUCTOS*
  +	Agregar producto.
  +	Mostrar productos.
  +	Modificar producto.
  +	Eliminar producto

*GESTION DE CLIENTES*
  +	Agregar cliente.
  +	Mostrar todos los clientes.
  +	Modificar cliente.
  +	Eliminar cliente.

*GESTION DE CARRITO*
  +	Agregar producto.
  +	Quitar producto.
  +	Ver carrito.
  +	Finalizar compra.
    + Sin descuento.
    + Descuento fijo.
    + Descuento porcentual.
  +	Top 5 precios mas caros.
  +	Ver cantidad de stock por categoria.

*GESTION DE PEDIDO*
  +	Mostrar pedido.
  +	Mostrar pedido por estado.
  +	Elevar estado de un pedido.

**Uso**
1.	Clonar repo.
2.	Configurar cadena de conexion en App.config.
3.	Ejecutar SQL para crear tablas: (Producto, Cliente, Carrito, CarritoItem, EstadoPedido, Pedido, PedidoItem).
4.	Compilar y ejecutar programa consola.
5.	Seguir menu para cargar, mostrar, modificar, borrar (Producto por consola).

**Base de Datos**
```sql
CREATE TABLE Producto (
    productoID INT IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    categoria VARCHAR(50) NOT NULL,
    precio DECIMAL(10,2) NOT NULL,
    stock INT NOT NULL
);

CREATE TABLE Cliente (
    clienteID INT IDENTITY(1,1) PRIMARY KEY,
    dNI CHAR(8) NOT NULL UNIQUE,
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL,
    telefono VARCHAR(20) NOT NULL,
    fechaRegistro DATETIME NOT NULL
);

CREATE TABLE Carrito (
    carritoID INT IDENTITY(1,1) PRIMARY KEY,
    clienteID INT NOT NULL
);

CREATE TABLE CarritoItem (
    carritoItemID INT IDENTITY(1,1) PRIMARY KEY,
    carritoID INT NOT NULL,
    productoID INT NOT NULL,
    cantidad INT NOT NULL,
    precioUnitario DECIMAL(10,2) NOT NULL
);

CREATE TABLE EstadoPedido (
    estadoID INT IDENTITY(1,1) PRIMARY KEY,
    descripcion VARCHAR(50) NOT NULL
);

CREATE TABLE Pedido (
    pedidoID INT IDENTITY(1,1) PRIMARY KEY,
    clienteID INT NOT NULL,
    estadoID INT NOT NULL,
    fecha DATETIME NOT NULL,
    total DECIMAL(10,2)
);

CREATE TABLE PedidoItem (
    pedidoItemID INT IDENTITY(1,1) PRIMARY KEY,
    pedidoID INT NOT NULL,
    productoID INT NOT NULL,
    cantidad INT NOT NULL,
    precioUnitario DECIMAL(10,2) NOT NULL
);

ALTER TABLE Carrito
ADD CONSTRAINT FK_Carrito_Cliente
FOREIGN KEY (clienteID) REFERENCES Cliente(clienteID);

ALTER TABLE CarritoItem
ADD CONSTRAINT FK_CarritoItem_Carrito
FOREIGN KEY (carritoID) REFERENCES Carrito(carritoID);

ALTER TABLE CarritoItem
ADD CONSTRAINT FK_CarritoItem_Producto
FOREIGN KEY (productoID) REFERENCES Producto(productoID);

ALTER TABLE Pedido
ADD CONSTRAINT FK_Pedido_Cliente
FOREIGN KEY (clienteID) REFERENCES cliente(ClienteID);

ALTER TABLE Pedido
ADD CONSTRAINT FK_Pedido_Estado
FOREIGN KEY (estadoID) REFERENCES EstadoPedido(estadoID);

ALTER TABLE PedidoItem
ADD CONSTRAINT FK_PedidoItem_Pedido
FOREIGN KEY (pedidoID) REFERENCES Pedido(pedidoID);

ALTER TABLE PedidoItem
ADD CONSTRAINT FK_PedidoItem_Producto
FOREIGN KEY (productoID) REFERENCES Producto(productoID);
```

**Agregar datos a Producto**
```sql
INSERT INTO Producto (Nombre, Categoria, Precio, Stock) VALUES 
('Notebook Lenovo IdeaPad', 'Electrónica', 850.00, 10);

INSERT INTO Producto (Nombre, Categoria, Precio, Stock) VALUES 
('Smartphone Samsung Galaxy', 'Electrónica', 650.00, 15);

INSERT INTO Producto (Nombre, Categoria, Precio, Stock) VALUES 
('Auriculares Sony WH-1000XM4', 'Electrónica', 300.00, 20);

INSERT INTO Producto (Nombre, Categoria, Precio, Stock) VALUES 
('Camiseta Nike', 'Ropa', 40.00, 50);

INSERT INTO Producto (Nombre, Categoria, Precio, Stock) VALUES 
('Zapatillas Adidas', 'Ropa', 120.00, 25);

INSERT INTO Producto (Nombre, Categoria, Precio, Stock) VALUES 
('Mochila Herschel', 'Accesorios', 80.00, 30);

INSERT INTO Producto (Nombre, Categoria, Precio, Stock) VALUES 
('Libro "C# Programming"', 'Libros', 35.00, 40);

INSERT INTO Producto (Nombre, Categoria, Precio, Stock) VALUES 
('Silla Gamer DXRacer', 'Muebles', 220.00, 12);

INSERT INTO Producto (Nombre, Categoria, Precio, Stock) VALUES 
('Mesa de Oficina', 'Muebles', 150.00, 8);

INSERT INTO Producto (Nombre, Categoria, Precio, Stock) VALUES 
('Reloj Inteligente Apple Watch', 'Electrónica', 400.00, 18);
```

**Agregar datos a EstadoPedido**
```sql
INSERT INTO EstadoPedido (descripcion) VALUES 
('Creado'),
('Confirmado'),
('Enviado'),
('Entregado');
```
