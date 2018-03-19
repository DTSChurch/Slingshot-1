SELECT DISTINCT
	-- Id --
	'100' AS [Id]
	-- Name --
	,'V5 Small Groups' AS [Name]
FROM
[Shelby].[SGOrg] AS gr

-- We are only grabbing the parent groups and not the grand-parent groups
WHERE gr.Levels = '1'
	AND gr.[Counter] != 27 -- Exclude empty SECURITY ALERT group

