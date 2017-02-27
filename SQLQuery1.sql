SELECT ot.OrdNo, p.ProdName, ol.Qty, p.ProdPrice
FROM OrderTbl ot
JOIN OrdLine ol ON ot.OrdNo = ol.OrdNo
JOIN Product p ON ol.ProdNo = p.ProdNo
WHERE CustNo = 'C0954327';
