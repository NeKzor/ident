# ident

 An algorithm which tries to find the choices that lead to the fastest solution for [Tron: Identity].

[Tron: Identity]: https://www.bithellgames.com/tron-identity

## Does language matter?

|Language|Best solution (in keypresses)|
|---|:-:|
|English|1294|
|Chinese (Simplified)|1294|
|Chinese (Traditional)|1294|
|French|1294|
|German|1294|
|Italian (Italy)|1294|
|Japanese|1294|
|Korean|1294|
|Spanish (Castilian)|1294|

## Caveats

* The algorithm cannot predict consequences of "worse" choices with a better outcome.
* It currently takes minutes to get a result.

## Run locally

* Get [AssetRipper] and extract `level1` file into the `data` folder.
* Get [dotnet] SDK 7.0
* Run with `./run.sh` or `.\run.ps1`

[AssetRipper]: https://github.com/AssetRipper/AssetRipper/releases
[dotnet]: https://dotnet.microsoft.com/en-us/download/dotnet/scripts

## TODO

* Can we optimize for performance and for resources?
* Count non-text based keypresses
* Create a tool assisted speedrun
