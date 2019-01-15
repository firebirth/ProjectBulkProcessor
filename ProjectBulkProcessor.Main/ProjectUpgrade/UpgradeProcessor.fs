module UpgradeProcessor

let upgradeProjects = 
    ProjectScanner.getProjectInfos
    >> Seq.map ProjectBuilder.buildProject
    >> Seq.iter (fun (p, i) -> p.Save(i.projectPath))