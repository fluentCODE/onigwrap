Provides interop between Onigmo and CLR/Mono managed code.

There are two pieces that make up the package:

* `onigwrap` - A C library that wraps Onigmo, greatly simplifying the interface for which we need to provide interop. This also greatly limits the flexibility of Onig, but for our use of this library, we didn't need any of that flexibility.
* `OnigRegex` - A C# class library that implements interop to onigwrap, and exposes a tidier interface to consumers.

Building
========

First, get a copy of Onigmo or Oniguruma.

* https://github.com/k-takata/Onigmo
* http://www.geocities.jp/kosako3/oniguruma/ (We don't actively test against oniguruma, but it should work.)

Copy oniguruma.h into the onigwrap folder, alongside onigwrap.c and onigwrap.h.

From here, the build steps diverge for each platform:

Mac
---

Configure and build onig. The defaults should work, but Mono on Mac is usually 32 bit by default, so we'll add in the `-m32` flag.

`./configure "CFLAGS=-m32"`

`make`

Copy .libs/libonig.a to the onigwrap folder.

Now we build onigwrap:

`clang -m32 -dynamiclib -L. -lonig -o libonigwrap.dylib onigwrap.c`

Take the dylib and put it alongside your binary.

Windows
-------

Build and configure onig. Copy the win32/Makefile and win32/config.h to onig's root directory and run `nmake`. If you're building Onig as 64 bit, you'll need to edit the Makefile and add /MACHINE:X64 to the LINKFLAGS

Copy onig\_s.lib and oniguruma.h to the onigwrap folder.

Build onigwrap:

`cl.exe /DONIG_EXTERN=extern /D_USRDLL /D_WINDLL onigwrap.c /link /LTCG onig_s.lib /DLL /OUT:onigwrap.dll`

Copy onigwrap.dll to the folder with your binary.

Linux
-----

Build and configure onig. We need to prepare onig for static linking though, so add `-fPIC` to the CFLAGS. If your Mono version is 32bit, make sure to add -m32 to the CFLAGS too. (You may need to install a package like `gcc-multilib` to make the build work with -m32.)

`./configure "CFLAGS=-fPIC"`

Copy .libs/libonig.a to the onigwrap folder.

Build onigwrap:

`gcc -shared -fPIC onigwrap.c libonig.a -o libonigwrap.so`

Copy libonigwrap.so alongside your binary.
