all:	exe

pkgs:
	pip install -r requirements.txt

exe: 
	pyinstaller -F pybartool.py

run:	exe
	./dist/pybartool

clean:
	rm -r -f dist
	rm -r -f build

.PHONY:	venv exe pkgs