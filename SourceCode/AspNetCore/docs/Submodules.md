Submodules
==========

This repository composes ASP.NET Core repos from a collection of many submodules.
Working with submodules in git requires using commands not used in a normal git workflow.
Here are some tips for working with submodules.

For full information, see the [official docs for git submodules](https://git-scm.com/book/en/v2/Git-Tools-Submodules).

## Fundamental concept

The parent repo (aspnet/AspNetCore) stores two pieces of info about each submodule.

1. Where to clone the submodule from. This is stored in the .gitmodules file
2. The commit hash of the submodule to use.

This means you cannot commit a submodule's branch or a tag to the parent repo.
Other info may appear in the .gitmodules file, but it is only used when attempting to
[update a submodule.](#updating-submodules)

## Cloning

By default, submodules will not be present. Use `--recursive` to clone all submodules.

    git clone https://github.com/aspnet/AspNetCore.git --recursive

If you have already cloned, run this to initialize all submodules.

    git submodule update --init

## Pulling updates

When you execute `git pull`, submodules do not automatically snap to the version
used in new commits of the parent repo. Update all submodules to match the parent repo's
expected version by running this command.

    git submodule update

## Executing a command on each submodule

    git submodule foreach '<command>'

For example, to clean and reset each submodule:

    git submodule foreach 'git reset --hard; git clean -xfd'

## Updating submodules

Updating all submodules to newer versions can be done like this.

    git submodule update --remote

Updating just one subumodule.

    git submodule update --remote modules/EntityFrameworkCore/

This uses the remote url and branch info configuration stored in .gitmodules to pull new commits.
This does not guarantee the commit is going to be a fast-forward commit.

## Diff

You can see which commits have changed in submodules from the last parent repo commit by adding `--submodule` to git-diff.

    git diff --submodule

## Saving an update to a submodule

To move the parent repo to use a new commit, you must create a commit in the parent repo
that contains the new commit.

    git submodule update --remote modules/KestrelHttpServer/
    git add modules/KestrelhttpServer/
    git commit -m "Update Kestrel to latest version"

## PowerShell is slow in aspnet/AspNetCore

Many users have post-git, and extension that shows git status on the prompt line. Because `git status` with submodules
on Windows is very slow, it can make PowerShell unbearable to use.

To workaround this, disable checking git-status for each prompt.
```ps1
$GitPromptSettings.EnableFileStatus = $false
```
You can disable this permanently by adding to your `$PROFILE` file. (`notepad $PROFILE`)
