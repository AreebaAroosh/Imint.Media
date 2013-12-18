param($installPath, $toolsPath, $package, $project)

$player = $project.ProjectItems.Item("Configuration").ProjectItems.Item("Player")
ForEach ($item in $player.ProjectItems)
		$item.Properties.Item("CopyToOutputDirectory").Value = 1