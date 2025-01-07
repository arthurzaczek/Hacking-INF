Hacking-INF
================================

Hacking-INF is a platform to learn programming languages easily. Many small examples will help you to put your focus on the details.

Features
--------

* Examples can be leanred directly in the browser. No IDE needed.
* Examples are checked instantly.
* Supports any programming language.

Usage
-----

To see [Hacking-INF](https://hacking-inf.technikum-wien.at/home) in action, got to https://hacking-inf.technikum-wien.at/home

For more information about how to setup examples, please see the [Hacking-INF-Demo-Examples](https://git-inf.technikum-wien.at/INF/Hacking-INF-Demo-Examples) repository for details.

Development
-----------

To setup a linux Environment please execute the following additional steps:

1. install Rider
2. install the latest tsc compiler
3. exec `npm install`
4. exec `tsc --noStrictGenericChecks`
5. compile with Rider
6. exec `sudo cp ./Hacking-INF/bin/lib/linux/x86_64/libgit2-*.so /lib`

Copyright and License
---------------------

Copyright (C) 2017 Arthur Zaczek at the [UAS Technikum Wien](http://www.technikum-wien.at/)

Licensed under the [GPL V3](http://www.gnu.org/licenses/gpl-3.0.txt)

External libraries
------------------

*Hacking-INF* depends on external libraries. Please see their license aggreement for more details. Every library license is, as far as I can see, compatible with the [GPL V3](http://www.gnu.org/licenses/gpl-3.0.txt).

Building 2025
------------------

~~~~~~~~~~~~~~~~
nvm install 8.9.0
nvm use 8.9.0
npm install -g @angular/cli@6
~~~~~~~~~~~~~~~~

Yes, very old versions