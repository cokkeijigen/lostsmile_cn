@echo off

set "OUT_DIR=.\\build"
if not exist "%OUT_DIR%" ( mkdir %OUT_DIR% )
cd %OUT_DIR%
cmake -DCMAKE_BUILD_TYPE=Release ..
cmake --build . --config Release