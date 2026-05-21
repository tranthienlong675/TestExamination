Gentoo Linux is a Linux distribution built using the Portage package management system. Unlike a binary software distribution, the source code is compiled locally according to the user's preferences and is often optimized for the specific type of computer. Precompiled binaries are available for some packages, but only for systems using Glibc; no precompiled binaries are available for Musl based systems. Gentoo runs on a wide variety of processor architectures.

Gentoo package management is designed to be modular, portable, easy to maintain, and flexible. Gentoo describes itself as a meta-distribution because of its adaptability, in that the majority of its users have configurations and sets of installed programs which are unique to the system and the applications they use.

Gentoo Linux is named after the gentoo penguin, the fastest swimming species of penguin. The name was chosen to reflect the potential speed improvements of machine-specific optimizing, which is a major feature of Gentoo.

Gentoo Linux was initially created by Daniel Robbins as the Enoch Linux distribution. Its design philosophy was that of precompiled binaries which were tuned to the hardware and that only included required programs.[6] At least one version of Enoch was distributed under that name: version 0.75, in December 1999. An older release labeled "Enoch 0.5" can be found on the CD accompanying the August 1999 edition of the Danish computer magazine Alt om Data.

Daniel Robbins and the other contributors experimented with a fork of GCC known as EGCS, developed by Cygnus Solutions. It was at this point that "Enoch" was renamed "Gentoo" Linux. The modifications to EGCS eventually became part of the official GCC (version 2.95); Gentoo and other Linux distros benefited from similar speed increases.

After problems with a bug on his own system, Robbins halted Gentoo development and switched to FreeBSD for several months, later saying, "I decided to add several FreeBSD features to make our autobuild system (now called Portage) a true next-generation ports system."

Gentoo Linux 1.0 was released on March 31, 2002. In 2004, Robbins set up the non-profit Gentoo Foundation, transferred all copyrights and trademarks to it, and stepped down as chief architect of the project.

The current board of trustees is composed of five members who were announced (following an election) on March 2, 2008.[14] The seven-member Gentoo Council oversees related technical issues and policies. The Gentoo Council members are elected annually, for a period of one year, by the active Gentoo developers. When a member of the Council retires, the successor is voted into place by the existing Council members.

The Gentoo Foundation is a domestic non-profit corporation, registered in the State of New Mexico. In late 2007, the Foundation's charter was revoked, but by May 2008 the State of New Mexico declared that the Gentoo Foundation, Inc. had returned to good standing and was free to do business.

The creator of Gentoo, Daniel Robbins, left the project in both 2004 and 2007 due to conflicts with other developers.

Gentoo is aimed at Linux users who prefer a high degree of control over the software installed and running on their computer. Users who invest time in configuring and optimizing a Gentoo system can build efficient desktops and servers. Gentoo supports building a Linux kernel tailored to specific hardware. It provides detailed control over which services are installed and running, including the option to choose between systemd or OpenRC as the default init system, among other possibilities. Memory usage may also be reduced compared to some other distributions by omitting unnecessary kernel features and services.

Gentoo's package repositories provide a large collection of software. Each package contains details of any dependencies, so only the minimum set of packages need to be installed. Optional features of individual packages, such as whether they require LDAP or Qt support, can be selected by the user and any resulting package requirements are automatically included in the set of dependencies.

Gentoo itself does not have a default look and feel, hence installed packages usually appear as their authors intended.

Portage is Gentoo's software distribution and package management system. The original design was based on the ports system used by the Berkeley Software Distribution (BSD) operating systems. The Gentoo repository contains over 19,000 packages.

A single invocation of portage's emerge command can update the local copy of the Gentoo repository, search for a package, or download, compile, and install one or more packages and their dependencies. The built-in features can be set for individual packages, or globally, with so-called "USE flags".

Pre-compiled binaries are provided for some applications with long build times, such as LibreOffice and Mozilla Firefox, but users lose the ability to customize optional features. There are configuration options to reduce compiling times, such as by enabling parallel compiling or using pipes instead of temporary files. Package compiling may also be distributed over multiple computers. Additionally, the user may be able to mount a large filesystem in memory to further speed up the process of building packages. Some approaches have drawbacks and are not enabled by default. When installing the same package on multiple computers with sufficiently similar hardware, the package may be compiled once and a binary package created for quick installation on the other computers.

On December 29, 2023, it was announced that Gentoo will offer binary packages for download and direct installation. For most architectures, this will be limited to the core system and weekly updates. For amd64 and arm64 however the availability of binary packages reaches over 20 GB.

As Gentoo is a source-based distribution with a repository describing how to build the packages, adding instructions to build on different machine architectures is particularly easy.

Originally built on the IA-32 architecture, Gentoo has since been ported to many others. It is officially supported and considered stable on IA-32, x86-64, PA-RISC, 32-bit and 64-bit PowerPC, 64-bit SPARC, DEC Alpha, and 32- and 64-bit ARM architectures. It is also officially supported but considered in-development state on MIPS, PS3 Cell Processor, System Z/s390. Official support for 32-bit SPARC, SuperH and Itanium have been dropped.

Portability towards other operating systems, such as those derived from Berkeley Software Distribution (BSD), including macOS, is under active development by the Gentoo/Alt project. The Gentoo/FreeBSD project already has a working guide based on FreeSBIE, while Gentoo/NetBSD, Gentoo/OpenBSD and Gentoo/DragonFly are being developed. A project exists to get Portage working on OpenSolaris. An experimental port to GNU Hurd was released on April 1, 2026.

It is also possible to install a Gentoo Prefix (provided by a project that maintains alternative installation methods for Gentoo) in a Cygwin environment on Windows, but this configuration is experimental.

Gentoo may be installed in several ways. The most common is to use the Gentoo minimal CD with a stage3 tarball (explained below). As with many Linux distributions, Gentoo may be installed from almost any Linux environment, such as another Linux distribution's Live CD, Live USB, or Network Booting using the "Gentoo Alternative Install Guide". A normal install requires a connection to the Internet, but a network-less install guide exists.

On April 3, 2022, it was announced that there would be a new official image with a GUI, called the LiveGUI image. This can be installed onto installation media such as a USB drive or a dual-layer DVD. It includes a large selection of software, including the KDE Plasma 6 desktop environment, image editors, office software, system administration, and installation tools.

Previously, Gentoo supported installation from stage1 and stage2 tarballs. The Gentoo Foundation no longer recommends this usage; stage1 and stage2 are now meant only for Gentoo developers.

Following the initial install steps, the Gentoo Linux install process in the Gentoo Handbook describes compiling a new Linux kernel. This process is generally not required by other Linux distributions. Although the installation requires significantly more configuration than most mainline distributions, Gentoo provides documentation and tools such as its stage3 tarball and distribution kernels to simplify the process. In addition, users may also use an existing kernel known to work on their system by simply copying it to the boot directory, or installing one of the provided pre-compiled kernel packages, and updating their bootloader. Support for installation is provided on the Gentoo forum, Reddit, and IRC.

A Live USB of Gentoo Linux can be created manually, by using various tools, or with dd as described in the handbook. 
