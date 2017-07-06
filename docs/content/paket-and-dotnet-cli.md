# Paket and .NET Sdk (dotnet cli and MSBuild 15)

Paket provides support for the new [.NET Sdk](https://github.com/dotnet/sdk) projects that are used by [dotnet cli](https://github.com/dotnet/cli) (running with [.NET Core](https://github.com/dotnet/core)) or by MSBuild 15 (Visual Studio 2017 and mono 5).

The general workflow is not much different to using Paket with traditional .NET projects which it is described in the ["Getting started" tutorial](getting-started.html). 

## Setup

### Downloading Paket's Bootstrapper

For dotnet cli to work properly Paket needs to be used in ["magic mode"](bootstrapper.html#Magic-mode).

1. Create a `.paket` directory in the root of your solution.
1. Download the latest [`paket.bootstrapper.exe`](https://github.com/fsprojects/Paket/releases/latest)
   into that directory.
1. Rename `.paket/paket.bootstrapper.exe` to `.paket/paket.exe`.
1. Commit `.paket/paket.exe` into your repository.
1. After the first `.paket/paket.exe` call Paket will create a couple of files in `.paket` - commit those as well.

There are already a couple of dotnet templates available that will ship with Paket set up properly. In this case you don't need to setup the bootstrapper manually.

### Specifying dependencies

Create a [`paket.dependencies` file](dependencies-file.html) in your project's
root and specify all your dependencies in it. 

As usual, to create an empty `paket.dependencies` file, just run

```sh
.paket/paket.exe init
```

This step is absolutely the same as with traditional .NET projects.

### Installing dependencies

Install all required packages with:

```sh
.paket/paket.exe install
```

This step is absolutely the same as with traditional .NET projects.

### Installing dependencies into projects

Like with traditional .NET projects you also need to put a [`paket.references` files](references-files.html) alongside your MSBuild project files.

After `paket.references` files are created, to update your existing projects, just run `dotnet restore` (see [restoring packages](paket-and-dotnet-cli.html#Restoring-packages)).

In contrast to traditional .NET projects Paket will not generate .dll references into your project files. 
Instead it will only generate a single line:

```xml
<Import Project="..\..\.paket\Paket.Restore.targets" />
```

This hook will tell the .net sdk to restore packages via Paket's restore mechanism. 

A nice benefit is that your project files are now much cleaner and don't contain all the package .dll references.

### Restoring packages

**NOTE** changed from traditional .NET behaviour

In traditional .NET projects you were used to call the [paket restore](paket-restore.html) command from the root of your solution.

- With dotnet cli you can now call:

    ```sh
    dotnet restore
    ```

- With MSBuild 15 (Developer Command Prompt for VS2017 or mono 5) you can now call:

    ```sh
    msbuild /t:Restore
    ```

As a note, this will call `paket restore` under the hood.

So this step integrates well into the new `.NET Sdk` philosophy.

It also works automatically where auto-restore is supported. Like in Visual Studio 2017, where if you open a solution with paket set up properly then Visual Studio's background build will restore paket dependencies automatically.

### Updating packages

If you want to update packages you can use the [`paket update` command](paket-update.html):

```sh
.paket/paket.exe update
```

This step is absolutely the same as with traditional .NET projects.

### Creating packages

If you want to create NuGet packages you can continue to use the [paket pack](paket-pack.html) command and [template files](template-files.html).

Or you can use the new `Pack` support in the `.NET Sdk`, that will take project information from your MSBuild project and dependency information from the [`paket.references` file](references-files.html).

- With dotnet cli you can now call:

    ```sh
    dotnet pack
    ```

- With MSBuild 15 (Developer Command Prompt for VS2017 or mono 5) you can now call:

    ```sh
    msbuild /t:Pack
    ```

As a note for `.NET Sdk`, all info for pack can be customized (for example `Author`) for automation as:

- msbuild property in the project `<Author>Nigel Sheldon</Author>` or defined inside an imported files.
- env var `Author=Nigel Sheldon`
- command line arg, passing `/p:Author="Nigel Sheldon"`

### Converting from NuGet

The NuGet conversion process is identical to traditional .NET projects you can read about it in the [`Convert from NuGet` tutorial](convert-from-nuget-tutorial.html).

