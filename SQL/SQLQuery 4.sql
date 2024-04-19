/*
AS
BEGIN
SELECT TOP(10) *
FROM Products
ORDER BY Products.UnitPrice DESC
END

EXEC TenMostExpensiveProducts
	@OrderID= 10248	,@ProductID= 60	,@UnitPrice= 10, @Quantity= 12	,@Discount	= 0.10

SELECT * FROM [Order Details] 
	@OrderID	INT,
	@ProductID	INT,
	@UnitPrice	MONEY,
	@Quantity	SMALLINT,
	@Discount	REAL
AS
BEGIN
	UPDATE	[Order Details] 
	SET		UnitPrice = @UnitPrice,
			Quantity = @Quantity,
			Discount = @Discount
	WHERE	OrderID	= @OrderID
	AND		ProductID = @ProductID
END

EXEC UpdateOrderDetails
	@OrderID	= 10252
	,@ProductID	= 60
	,@UnitPrice	= 22
	,@Quantity	= 10
	,@Discount	= 0.058