# Installation Guide for UniConsole

## Download a Release

Click [here](https://github.com/lGruenzweilHTL/UniConsole/releases) to view all the releases of `UniConsole`. You can choose any one you want, but the latest one is recommended.

When you click on the release of your choice you will see the files that the release contains.


![Release example](Images/Release-Example.png)

**Core.zip** contains the scripts that `UniConsole` needs to work properly.

**Samples.zip** is a collection of samples that show how `UniConsole` works.

> [!IMPORTANT]
> You need to download **Core.zip** for the release to work correctly.
> If you want to, you can also download **Samples.zip**, but it is not required.

## Install Dependencies

`UniConsole` requires certain **Unity Packages** to be installed to work properly.

Luckily, the only Package you need to install is `TextMeshPro`.

#### TextMeshPro

TextMeshPro (or TMP for short) is a very central part of Unity's UI System. It uses Meshes to create sharp, high-resolution Texts.

Installation of TMP is very simple. Right click in the **Hierarchy window** and under `UI`, select any option ending in `TextMeshPro`.

![Text Mesh Pro](Images/TMP.png).

This will open the **TMP Importer**

![TMP Importer](Images/TMPImport.png)

Click on `Import TMP Essentials` and the Importer will automatically install TextMeshPro for you.

Once the Importer has finished installing, you are ready to set up `UniConsole` in Unity.

## Setup in Unity

Extract the file(s) you downloaded and drag them into your unity project.

Open your Unity Project and navigate to `<PathOfCore>/Prefabs`, with *PathOfCore* meaning the Path where you dragged the **Core folder** to.

You will see a Prefab called `Terminal`.

![Prefabs](Images/Prefab.png)

First, add a **Canvas** into your scene by right clicking in the **Hierarchy window** and choosing `UI/Canvas`.
Drag the Prefab into your **Canvas**. You should see the terminal appear in the bottom right corner of your **Game view**.

![Hierarchy](Images/Hierarchy.png)

![Game view](Images/Gameview.png)

If you start your game now, you can try the Terminal by typing some commands.

## Finished

Congratulations. You are all done with installing `UniConsole`. 

Thank you for using my tool!