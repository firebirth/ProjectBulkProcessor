module UpgradeProcessor

let upgradeProjects =
    ProjectScanner.getProjectInfos
    >> List.map ProjectBuilder.buildProject
    >> List.iter (fun (p, i) -> p.Save(i.projectPath))
