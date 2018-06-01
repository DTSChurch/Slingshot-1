/*
Run this statement against the Shelby V5 db.
The active groups are the ones where:
	L1Active = -1 and
	L2Active = -1 and
	L3Active = -1 and
	L4Active = -1 and
	L5Active = -1

(This is the pattern for Bright Christian Church's Shelby V5.)
(NOTE: If this pattern holds for other Shelby V5's, then this pattern should be used for a single script that will mark the appropriate groups inactive.)
*/
select
 O.Counter as OrgCounter,
 G.Counter as GroupCounter,
 L1.Counter as L1Counter,
 L2.Counter as L2Counter,
 L3.Counter as L3Counter,
 L4.Counter as L4Counter,
 L5.Counter as L5Counter,
 Organization = O.Descr,
 Level05Name = isnull(L5.Descr, O.Descr),
 Level05ID = isnull(L5.IDLevel, upper(cast(O.Descr as varchar(8)))),
 Level04Name = isnull(L4.Descr, O.Descr),
 Level04ID = isnull(L4.IDLevel, upper(cast(O.Descr as varchar(8)))),
 Level03Name = isnull(L3.Descr, O.Descr),
 Level03ID = isnull(L3.IDLevel, upper(cast(O.Descr as varchar(8)))),
 Level02Name = isnull(L2.Descr, O.Descr),
 Level02ID = isnull(L2.IDLevel, upper(cast(O.Descr as varchar(8)))),
 Level01Name = isnull(L1.Descr, O.Descr),
 Level01ID = isnull(L1.IDLevel, upper(cast(O.Descr as varchar(8)))),
 GroupName = isnull(G.Descr, O.Descr),
 GroupID = isnull(G.IDGrp, upper(cast(O.Descr as varchar(8)))),
 RollHeading = case when G.RollHeading > '' then G.RollHeading when G.Descr > '' then G.Descr else O.Descr end,
 RoomNumber = case when G.RoomNu > '' then G.RoomNu else 'Not Assigned' end,
 L1.Active as L1Active,
 L2.Active as L2Active,
 L3.Active as L3Active,
 L4.Active as L4Active,
 L5.Active as L5Active
from
 Shelby.SGOrg as O left join
 Shelby.SGOrgGrp as G on O.Counter = G.SGOrgCounter left join
 Shelby.SGOrgLvl as L1 on G.SGOrgLvlCounter = L1.Counter left join
 Shelby.SGOrgLvl as L2 on L1.SGOrgLvlAbove = L2.Counter left join
 Shelby.SGOrgLvl as L3 on L2.SGOrgLvlAbove = L3.Counter left join
 Shelby.SGOrgLvl as L4 on L3.SGOrgLvlAbove = L4.Counter left join
 Shelby.SGOrgLvl as L5 on L4.SGOrgLvlAbove = L5.Counter
 

 --Use the select statement below to generate a comma delimited list of group ForeignId's (in Rock). 
 --Adjust the where clause.
 --Use the list in a query where clause.
/*
 select
 STUFF((SELECT ', ' + CAST(G.Counter AS VARCHAR)
	       from
			 Shelby.SGOrg as O left join
			 Shelby.SGOrgGrp as G on O.Counter = G.SGOrgCounter left join
			 Shelby.SGOrgLvl as L1 on G.SGOrgLvlCounter = L1.Counter left join
			 Shelby.SGOrgLvl as L2 on L1.SGOrgLvlAbove = L2.Counter left join
			 Shelby.SGOrgLvl as L3 on L2.SGOrgLvlAbove = L3.Counter left join
			 Shelby.SGOrgLvl as L4 on L3.SGOrgLvlAbove = L4.Counter left join
			 Shelby.SGOrgLvl as L5 on L4.SGOrgLvlAbove = L5.Counter
			 where  L1.Active = 0 and L2.Active = 0
			FOR XML PATH('')), 
                            1, 1, '') as GroupInactiveForeignIds
*/


--Use this against Rock to view the Shelby Groups in Rock
--Useful for troubleshooting
/*
select 
 PG.Name as TopName
, PG.Id as TopNameId
, PG2.Name as MidName
, PG2.Id as MidNameId
, PG2.IsActive as MidNameIsActive
, PG2.ForeignId as MidNameForeignId
, PG3.Name as LowerName
, PG3.Id as LowerNameId
, PG3.IsActive as LowerNameIsActive
from [Group] PG
LEFT OUTER JOIN [Group] PG2 ON PG.Id = PG2.ParentGroupId
LEFT OUTER JOIN [Group] PG3 ON PG2.Id = PG3.ParentGroupId
where PG.GroupTypeId = 33 and  PG3.IsActive = 1 
order by MidName
*/
