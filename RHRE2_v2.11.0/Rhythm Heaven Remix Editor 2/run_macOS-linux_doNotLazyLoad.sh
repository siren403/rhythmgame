#!/bin/bash

java -version

printf "\nYou NEED Java 1.8, at least. If the version above is not 1.8+, or if you get an UnsupportedClassVersionError, update Java.\n"

java -jar -Dfile.encoding=UTF8 RHRE.jar --force-load-lazy-sounds

read -n1 "Press any key to continue..."
