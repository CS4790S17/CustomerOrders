SELECT o.OrdNo, l.ProdNo, p.ProdName, l.QTY, p.ProdPrice
From OrderTbl o INNER JOIN OrdLine l
ON o.OrdNo = l.OrdNo
INNER JOIN Product p
ON l.ProdNo = p.ProdNo
WHERE l.OrdNo = '1'



SELECT o.OrdNO, l.ProdNo, p.ProdName, l.Qty, p.ProdPrice
FROM OrderTbl o
INNER JOIN OrdLine l
ON o.OrdNo = l.OrdNo
INNER JOIN Product p
ON l.ProdNo = p.ProdNo
WHERE l.OrdNo = 'O1116324'


