# SpatialOS SDK for Unity Wizards Tutorial

**New from October 2018: The SpatialOS GDK for Unity**<br/>
Check out the alpha release of our [SpatialOS Game Development Kit (GDK) for Unity](https://docs.improbable.io/unity/latest/welcome). Using the Unity Entity Component System (ECS), the GDK is the next evolution in developing SpatialOS games in Unity. The SpatialOS GDK for Unity is designed to replace the SpatialOS SDK for Unity and we recommend using it over using the SDK for new game projects. See our [blog post anouncement](https://improbable.io/games/blog/spatialos-gdk-for-unity-launch?utm_medium=docs&utm_source=onboarding&utm_campaign=spatialos-gdk-unity-launch&utm_content=10-oct) for more information.

---

> **Important notice:** We are retiring the Wizards project! This repository will remain in place, but we no longer recommend Wizards as part of your path to learning about SpatialOS. For a better experience, get hands-on with the [Pirates tutorial](https://github.com/spatialos/PiratesTutorial), learn the [core concepts of SpatialOS](https://docs.improbable.io/reference/latest/shared/concepts/spatialos), and read about tools for [running a live game](https://docs.improbable.io/reference/latest/shared/operate/inspector).



![Wizards Logo](wizards-logo.jpg)

- [Guide](https://docs.improbable.io/reference/13.0/shared/get-started/tour) (Website docs)

*****

### Introduction

This repository contains a demo project built with [SpatialOS](https://docs.improbable.io/reference/13.0/shared/concepts/spatialos).
It demonstrates how to use SpatialOS to build a large and compelling simulated world.

The project serves as a starting point for the [hands-on SpatialOS product demo](https://docs.improbable.io/reference/13.0/shared/get-started/tour).

The main documentation for SpatialOS can be found [here](https://spatialos.improbable.io/docs/reference/13.0/index).

If you run into problems, or want to give us feedback, please visit the [SpatialOS forums](https://forums.improbable.io/).

---

**New from June 2018: The SpatialOS Unity GDK**<br/>
Check out the pre-alpha release of our new Unity GDK: [source code on GitHub](https://github.com/spatialos/UnityGDK). Using the Unity Entity Component System (ECS), the GDK is the next evolution in SpatialOS Unity game development. See our [blog post on ECS-powered multiplayer](https://improbable.io/games/blog/unity-gdk-our-first-steps) for more information.
<br/>
<br/>
**Note:** The pre-alpha GDK version is available for evaluation and feedback only. It's not yet game-development ready.

---

#### To use the repository

* Make sure you have access to SpatialOS
* Install dependencies for [Windows](https://docs.improbable.io/reference/13.0/shared/get-started/setup/win) or [Mac](https://spatialos.improbable.io/docs/reference/13.0/shared/get-started/setup/mac)
* Clone the repo: `git clone https://github.com/spatialos/wizards`
* Move into the directory: `cd wizards`
* Build the project: `spatial worker build`
* To run locally:
* Run: `spatial local launch`
* Connect multiple player clients: `spatial local worker launch UnityClient default`
* To deploy in the cloud:
* Update spatialos.json: edit the "name" parameter from `your_project_name_here` to your spatialOS project name and save it
* Upload the assembly: run `spatial cloud upload my_wizards_assembly`
* Launch the deployment: run `spatial cloud launch my_wizards_assembly default_launch.json my_wizards_deployment --snapshot=./snapshots/default.snapshot`

<br/>

---
*Copyright (C) 2018 Improbable Worlds Limited. All rights reserved.*
