SELECT Pseudo FROM Personnes;
SELECT * FROM Logiciels WHERE Code = 'ANATOMIA';
SELECT * FROM Modules WHERE Code = 'RADIO' AND CodeLogiciel = 'ANATOMIA';
SELECT * FROM Versions WHERE Numero = 6 AND CodeLogiciel = 'ANATOMIA';

SELECT * FROM Versions WHERE Numero = 6 AND CodeLogiciel = 'ANATOMIA';