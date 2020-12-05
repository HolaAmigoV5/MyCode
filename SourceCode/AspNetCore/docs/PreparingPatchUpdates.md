Preparing new servicing updates
===============================

In order to prepare this repo to build a new servicing update, the following changes need to be made.

* Increment the patch version in the [version.props](/version.props) file in the repository root.

    ```diff
    -  <AspNetCorePatchVersion>7</AspNetCorePatchVersion>
    +  <AspNetCorePatchVersion>8</AspNetCorePatchVersion>
    ```

* Update the package baselines. This is used to ensure packages keep a consistent set of dependencies between releases.
  See [eng/tools/BaselineGenerator/](/eng/tools/BaselineGenerator/README.md) for instructions on how to run this tool.

* Update the list of packages in [eng/PatchConfig.props](/eng/PatchConfig.props) to list which packages should be patching in this release.
