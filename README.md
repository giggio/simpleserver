# SimpleServer

[![Build app](https://github.com/giggio/simpleserver/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/giggio/simpleserver/actions/workflows/build.yml)
[![codecov](https://codecov.io/gh/giggio/simpleserver/branch/main/graph/badge.svg?token=O8UDJRBFR1)](https://codecov.io/gh/giggio/simpleserver)

A simple server command line tool to run a static backend. Written in C#, cross platform.

It is cross platform, has around 30MB, and does not require .NET ou .NET Framework to run.

## Running

Download the version to your OS (see bellow) and run it like this:

```bash
simpleserver [<path>] [options]
```

If you run it without arguments it will serve the current running directory (pwd/cwd), bound
to all network interfaces (`0.0.0.0`) on port `8080` and without logging to stdout. These are
all configurable and there are many other options.

| option                       | type   | default value | description                                               |
| ---------------------------- | ------ | ------------- | --------------------------------------------------------- |
| `-i`, `--index`              | switch |               | Use a default index file when a file is not found.        |
| `--interface <interface>`    | string | `0.0.0.0`     | Network interface (IP or hostname) to bind the server to. |
| `-p, --port <port>`          | u16    | `8080`        | Port to run the server at.                                |
| `--index-name <index-name>`  | string | `index.html`  | Default index file name.                                  |
| `-v`, `--verbose`            | switch |               | Show verbose output.                                      |
| `-s`, `--simple-paths`       | switch |               | Use simple paths. Allows to serve 'file.html' as '/file'. |
| `-l`, `--log-http`           | switch |               | Log http output to stdout.                                |
| `-d`, `--directory-browsing` | switch |               | Enable directory browsing.                                |

You can see this options in the command line by typing `simpleserver --help`.

For example, to run an SPA, you would want to serve every static file and serve `index.html` for every requested route
that was not matched to a file. Like this:

```bash
simpleserver -i
```

To change the file, you'd also supply another option:

```bash
simpleserver -i --index-name default.html
```

### Incompatible options

You can't run directory browsing and index file at the same time. If you try, directory browsing will be disabled.

You can't run simple paths and directory browsing at the same time. If you try, simple paths will be disabled.

### Running the framework dependent .dll

The framework dependent simpleserver.dll can be run where you already have the .NET Runtime installed. You'll need the
.NET 6 runtime, and run it like this:

```bash
dotnet simpleserver.dll [<path>] [options]
```

You will need all the files that are in the .tgz, decompress it to a directory and run it from there.

## Installing

Download an artifact from the latest
[release](https://github.com/giggio/simpleserver/releases/latest)
and add it to your path.

There are artifacts for Windows and Linux (x86, ARM and Musl). The Linux ones are
are dynamic binaries, they can't run on distroless containers (`FROM scratch`).

* `simpleserver-linux-arm` - Linux ARM
* `simpleserver-linux-musl-x64` - Linux x64 Musl (Alpine etc)
* `simpleserver-linux-x64` - Linux ARM
* `simpleserver.exe` - Windows x64
* `simpleserver.tgz` - Cross platform, [framework dependent](https://docs.microsoft.com/en-us/dotnet/core/deploying/#publish-framework-dependent)

The `.pdb` files are symbol files and are only needed for debugging, not running the application.

### Completions

This is written with `dotnet-suggest`. You'll need the .NET Sdk to enable completions. The detailed steps
[are outlined at the `command-line-api` project](https://github.com/dotnet/command-line-api/blob/main/docs/dotnet-suggest.md).

It works everywhere the .NET Sdk works. The supported shells are detailed on the above document and include Bash, Zsh and PowerShell.

## Contributing

Questions, comments, bug reports, and pull requests are all welcome.  Submit them at
[the project on GitHub](https://github.com/giggio/simpleserver).

Bug reports that include steps-to-reproduce (including code) are the
best. Even better, make them in the form of pull requests.

## Author

[Giovanni Bassi](https://github.com/giggio).

## License

Licensed under the MIT License.
