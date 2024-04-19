--Write a query to get a Product list (id, name, unit price) where current products cost less than $20. 
SELECT ProductID, ProductName, UnitPrice FROM Products WHERE  UnitPrice<20

--Write a query to get Product list (id, name, unit price) where products cost between $15 and $25 
SELECT ProductID, ProductName, UnitPrice FROM Products WHERE  UnitPrice BETWEEN 15 AND 25

--Write a query to get Product list (name, unit price) of above average price.  
SELECT ProductName, UnitPrice FROM Products WHERE  UnitPrice> (SELECT avg(UnitPrice) FROM Products)

--Write a query to get Product list (name, unit price) of ten most expensive products 
SELECT top 10 ProductName, UnitPrice FROM Products ORDER BY UnitPrice desc 

--Write a query to count current and discontinued products 
SELECT Discontinued, COUNT(Discontinued) as value FROM Products GROUP BY Discontinued

--Write a query to get Product list (name, units on order, units in stock) of stock is less than the quantity on order 
SELECT ProductName, UnitsOnOrder, UnitsInStock FROM Products WHERE UnitsInStock<UnitsOnOrder


SELECT
  COUNT(CASE WHEN  Discontinued= 0 then 1 ELSE NULL END) as "current",
   CASE WHEN  ( COUNT(CASE WHEN  Discontinued= 0 then 1 ELSE NULL END)  % 2 = 0) then 'yes' ELSE 'no' END as "divisible",
    COUNT(CASE WHEN Discontinued = 1 then 1 ELSE NULL END) as "discontinued",
	  CASE WHEN  (COUNT(CASE WHEN Discontinued = 1 then 1 ELSE NULL END)  % 2 = 0) then 'yes' ELSE 'no' END as "divisible",
	COUNT(Discontinued) as 'total'   
	 from Products