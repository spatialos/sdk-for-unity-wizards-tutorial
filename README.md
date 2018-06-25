# Wizards! (SpatialOS)

![Wizards Logo](wizards-logo.jpg)



- [Guide](https://docs.improbable.io/reference/13.0/shared/get-started/tour) (Website docs)
- GitHub repository: [github.com/spatialos/Wizards](https://github.com/spatialos/Wizards)

*****

### Introduction

This repository contains a demo project built with [SpatialOS](https://docs.improbable.io/reference/13.0/shared/concepts/spatialos).
It demonstrates how to use SpatialOS to build a large and compelling simulated world.

The project serves as a starting point for the [hands-on SpatialOS product demo](https://docs.improbable.io/reference/13.0/shared/get-started/tour).

The main documentation for SpatialOS can be found [here](https://spatialos.improbable.io/docs/reference/13.0/index).

If you run into problems, or want to give us feedback, please visit the [SpatialOS forums](https://forums.improbable.io/).

---

**New from June 2018: The SpatialOS Unity GDK**<br/>
Check out the pre-alpha release of our new Unity GDK: [source code on GitHub](https://github.com/spatialos.com/UnityGDK). Using the Unity Entity Component System (ECS), the GDK is the next evolution in SpatialOS Unity game development. See our [blog post on ECS-powered multiplayer](https://improbable.io/games/blog/unity-gdk-our-first-steps) for more information.
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
*Copyright (C) 2017 Improbable Worlds Limited. All rights reserved.*