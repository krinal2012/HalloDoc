create table Salesman
(Salesman_id "int" NOT NULL primary key,
Salesman_name varchar(20) not null,
Salesman_city varchar(20) not null,
Salesman_commission FLOAT  NOT NULL );

create table Customer1
(Customer_id "int" NOT NULL primary key,
Customer_name varchar(20) not null,
Customer_city varchar(20) not null,
Customer_grade "int" not null,
salesman_id "int"  NOT NULL references Salesman(Salesman_id) );

create table Orders1
(Order_no "int" NOT NULL primary key,
Pure_amount "int" not null,
Order_date date not null,
Customer_id "int" references Customer1(Customer_id) ,
Salesman_id "int" references Salesman(Salesman_id) );

insert into Salesman values(11,'Pranav','Karwar',0.10);
insert into Salesman values(24,'Prasanna','Bengalore',0.12);
insert into Salesman values(39,'Prajwal','Kodagu',0.16);
insert into Salesman values(44,'Pooja','Hubli',0.05);
insert into Salesman values(15,'Prokta','Mysore',0.15);

insert into Customer1 values(101,'Bhargav','Mysore',1,15);
insert into Customer1 values(206,'Ramya','Bengalore',3,24);
insert into Customer1 values(225,'Rajesh','Hubli',2,39);
insert into Customer1 values(324,'Ravi','Mangalore',5,44);
insert into Customer1 values(456,'Rajdeep','Belagavi',3,15);
insert into Customer1 values(501,'Raghu','Dharavad',4,39);
insert into Customer1 values(300,'Bhavya','Bengalore',1,15);

insert into Orders1 values(5,10000,'2020-03-25',101,11);
insert into Orders1 values(10,5000,'2020-03-25',456,15);
insert into Orders1 values(7,9500,'2020-04-30',225,44);
insert into Orders1 values(11,8700,'2020-07-07',324,24);
insert into Orders1 values(17,1500,'2020-07-07',206,39);

SELECT * From Customer1

--1 write a SQL query to find the salesperson and customer who reside in the same city. Return Salesman, cust_name and city
Select s.Salesman_name, c.Customer_name, c.Customer_city 
From Salesman s Join Customer1 c
ON s.Salesman_id=c.Salesman_id
Where s.Salesman_city=c.Customer_city

--2 write a SQL query to find those orders where the order amount exists between 500 and 2000. Return ord_no, purch_amt, cust_name, city
Select o.Order_no, o.Pure_amount, c.Customer_name, c.Customer_city
From Orders1 o Join Customer1 c
ON o.customer_id =c.customer_id
Where o.Pure_amount BETWEEN 500 AND 2000

--3 write a SQL query to find the salesperson(s) and the customer(s) he represents. Return Customer Name, city, Salesman, commission
Select  c.Customer_name, c.Customer_city,s.Salesman_name,s.Salesman_commission
From Salesman s Join Customer1 c
ON s.Salesman_id=c.Salesman_id

--4 Write a SQL query to find salespeople who received commissions of more than 12 percent from the company. Return Customer Name, customer city, Salesman, commission.
Select c.Customer_name, c.Customer_city, s.Salesman_name,s.Salesman_commission
From Salesman s Join Customer1 c
ON s.Salesman_id=c.Salesman_id
Where s.Salesman_commission>0.12

--5 write a SQL query to locate those salespeople who do not live in the same city where their customers live and have received a commission of more than 12% from the company.
-- Return Customer Name, customer city, Salesman, salesman city, commission
Select c.Customer_name, c.Customer_city, s.Salesman_name,s.Salesman_commission
From Salesman s Join Customer1 c
ON s.Salesman_id=c.Salesman_id
Where s.Salesman_commission>0.12 AND s.Salesman_city!=c.Customer_city

--6 write a SQL query to find the details of an order. Return ord_no, ord_date, purch_amt, Customer Name, grade, Salesman, commission
SELECT o.Order_no,o.Order_date,o.Pure_amount,c.Customer_name,s.Salesman_name, s.Salesman_commission 
FROM orders1 o
JOIN customer1 c
ON o.customer_id=c.customer_id 
JOIN salesman s
ON o.salesman_id=s.salesman_id;

--7 Write a SQL statement to join the tables salesman, customer and orders so that the same column of each table appears once and only the relational rows are returned. 
SELECT
	s.Salesman_id,
	s.Salesman_city,
	s.Salesman_name,
	s.Salesman_commission,
	c.Customer_id,
	c.Customer_name,
	c.Customer_grade,
	o.Order_no,
	o.Pure_amount,
	o.Order_date		
FROM Customer1 c
JOIN salesman s
  ON s.Salesman_id = c.Salesman_id
JOIN Orders1 o 
  ON o.Customer_id  = c.Customer_id;

--8 write a SQL query to display the customer name, customer city, grade, salesman, salesman city. The results should be sorted by ascending customer_id.
Select c.Customer_name, c.Customer_city,c.Customer_grade, s.Salesman_name,s.Salesman_city
From Salesman s Join Customer1 c
ON s.Salesman_id=c.Salesman_id
ORDER BY c.customer_id ASC;

--9 write a SQL query to find those customers with a grade less than 300. Return cust_name, customer city, grade, Salesman, salesmancity. The result should be ordered by ascending customer_id.
Select c.Customer_name, c.Customer_city,c.Customer_grade, s.Salesman_name,s.Salesman_city
From Salesman s Join Customer1 c
ON s.Salesman_id=c.Salesman_id
WHERE c.Customer_grade<300
ORDER BY c.customer_id ASC;

--10 Write a SQL statement to make a report with customer name, city, order number,order date, and order amount in ascending order according to the order date to 
--determine whether any of the existing customers have placed an order or not
Select c.Customer_name, c.Customer_city,o.Order_no,o.Order_date, o.Pure_amount 
FROM customer1 c
LEFT OUTER JOIN orders1 o
ON o.customer_id=c.customer_id 
ORDER BY Order_date ASC;

--11 Write a SQL statement to generate a report with customer name, city, order number,order date, order amount, salesperson name, and commission to determine if any of 
--the existing customers have not placed orders or if they have placed orders through their salesman or by themselves
Select c.Customer_name, c.Customer_city,o.Order_no,o.Order_date, o.Pure_amount, s.Salesman_name, s.Salesman_commission 
FROM customer1 c
LEFT OUTER JOIN orders1 o
ON c.customer_id=o.customer_id
LEFT OUTER JOIN salesman s
ON c.salesman_id=s.salesman_id; 

--12 Write a SQL statement to generate a list in ascending order of salespersons who work either for one or more customers or have not yet joined any of the customers
Select s.Salesman_name, c.Customer_name 
From Salesman s LEFT OUTER Join Customer1 c
ON s.Salesman_id=c.Salesman_id
ORDER BY s.salesman_id ASC; 

--13 write a SQL query to list all salespersons along with customer name, city, grade, order number, date, and amount.
Select s.*, c.Customer_name, c.Customer_city, c.Customer_grade, o.Order_no,o.Order_date, o.Pure_amount
From Salesman s 
LEFT OUTER Join Customer1 c
ON s.Salesman_id=c.Salesman_id
LEFT OUTER JOIN orders1 o
ON c.customer_id=o.customer_id

--14 Write a SQL statement to make a list for the salesmen who either work for one or more customers or yet to join any of the customers. The customer may have placed, 
--either one or more orders on or above order amount 2000 and must have a grade, or he may not have placed any order to the associated supplier.
Select s.Salesman_name, c.Customer_name, c.Customer_city, c.Customer_grade, o.Order_no,o.Order_date, o.Pure_amount
From Salesman s 
LEFT OUTER Join Customer1 c
ON s.Salesman_id=c.Salesman_id
JOIN orders1 o
ON c.customer_id=o.customer_id
WHERE Pure_amount>=2000 
AND c.Customer_grade is NOT NULL

Select s.Salesman_name, c.Customer_name, c.Customer_city, c.Customer_grade, o.Order_no,o.Order_date, o.Pure_amount
From Salesman s 
 Join Customer1 c
ON s.Salesman_id=c.Salesman_id
JOIN orders1 o
ON c.customer_id=o.customer_id
WHERE c.Customer_grade is NULL
UNION
Select s.Salesman_name, c.Customer_name, c.Customer_city, c.Customer_grade, o.Order_no,o.Order_date, o.Pure_amount
From Salesman s 
Join Customer1 c
ON s.Salesman_id=c.Salesman_id
JOIN orders1 o
ON c.customer_id=o.customer_id
WHERE Pure_amount>=2000 
AND c.Customer_grade is NOT NULL

--15 Write a SQL statement to generate a list of all the salesmen who either work for one or more customers or have yet to join any of them. The customer may have placed 
--one or more orders at or above order amount 2000, and must have a grade, or he may not have placed any orders to the associated supplier
SELECT * 
FROM Salesman s 
LEFT OUTER Join Customer1 c
ON s.Salesman_id=c.Salesman_id
RIGHT OUTER JOIN orders1 o
ON c.customer_id=o.customer_id
WHERE Pure_amount>=2000 
AND c.Customer_grade is NOT NULL

--16 Write a SQL statement to generate a report with the customer name, city, order no. order date, purchase amount for only those customers on the 
--list who must have a grade and placed one or more orders or which order(s) have been placed by the customer who neither is on the list nor has a grade.
Select c.Customer_name, c.Customer_city,o.Order_no,o.Order_date, o.Pure_amount 
FROM Customer1 C
RIGHT OUTER JOIN orders1 o
ON c.customer_id=o.customer_id
WHERE C.Customer_grade IS NOT NULL 

--17 Write a SQL query to combine each row of the salesman table with each row of the customer table
SELECT * 
FROM Salesman 
CROSS JOIN Customer1

--18 Write a SQL statement to create a Cartesian product between salesperson and customer.i.e. each salesperson will appear for all customers and vice versa for that salesperson who belongs to that city
SELECT * 
FROM salesman S 
CROSS JOIN Customer1 C 
WHERE s.Salesman_city=c.Customer_city

--19 Write a SQL statement to create a Cartesian product between salesperson and customer, i.e. each salesperson will appear for every customer and vice versa for those salesmen who belong to a city 
--and customers who require a grade
SELECT * 
FROM salesman S 
CROSS JOIN Customer1 C 
WHERE s.Salesman_city IS NOT NULL 
AND c.Customer_grade IS NOT NULL 

--20 Write a SQL statement to make a Cartesian product between salesman and customer i.e. each salesman will appear for all customers and vice versa for those salesmen who must belong to a city
--which is not the same as his customer and the customers should have their own grade
SELECT * 
FROM salesman S 
CROSS JOIN Customer1 C 
WHERE s.Salesman_city IS NOT NULL 
AND c.Customer_grade IS NOT NULL
AND s.Salesman_city!=c.Customer_city