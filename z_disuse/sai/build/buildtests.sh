#!/bin/bash

if [[ $# != 1 || $1 == *help ]]
then
  echo "usage: $0 regexp"
  echo "  Builds tests matching the regexp."
  echo "  The EIGEN_MAKE_ARGS environment variable allows to pass args to 'make'."
  echo "    For example, to launch 5 concurrent builds, use EIGEN_MAKE_ARGS='-j5'"
  exit 0
fi

TESTSLIST=""
targets_to_make=$(echo "$TESTSLIST" | grep -E "$1" | xargs echo)

if [ -n "${EIGEN_MAKE_ARGS:+x}" ]
then
  C:/Program Files/Microsoft Visual Studio/2022/Community/MSBuild/Current/Bin/amd64/MSBuild.exe $targets_to_make ${EIGEN_MAKE_ARGS}
else
  C:/Program Files/Microsoft Visual Studio/2022/Community/MSBuild/Current/Bin/amd64/MSBuild.exe $targets_to_make 
fi
exit $?

