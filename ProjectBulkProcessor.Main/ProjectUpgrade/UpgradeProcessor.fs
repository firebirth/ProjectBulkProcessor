module UpgradeProcessor

let upgradeProjects root =
    ProjectScanner.getProjectInfos root
    |> Seq.map ProjectBuilder.buildProject
    |> Seq.iter (fun (p, i) -> p.Save(i.projectPath))