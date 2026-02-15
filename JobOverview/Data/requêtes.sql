delete from Releases where CodeLogiciel = 'ANATOMIA' and NumeroVersion = 6

select * from Releases where CodeLogiciel = 'ANATOMIA' and NumeroVersion = 6


-- requete pour verifier que les taches et les travaux sont bien supprimes
select * from Taches t
left outer join Travaux tr on t.Id = tr.IdTache
where t.CodeLogiciel = 'ANATOMIA'


-- requete pour remettre le compteur auto-incrementé à 44
declare @id int
delete from Travaux where IdTache > 44
delete from Taches where Id > 44
select @id = max(Id) from Taches
DBCC CHECKIDENT ('Taches', RESEED, @id)