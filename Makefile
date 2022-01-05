PROGRAM_NAME=		MicroBarTo
VERSION= 			$(shell python -c "import microbarto; print(microbarto.__version__)")
DATETIME?=			$(shell date +'%y%m%d-%H%M%S')
BUILD_EXEFILE=		$(PROGRAM_NAME)-$(VERSION)-$(DATETIME).exe
RELEASE_EXEFILE=	$(PROGRAM_NAME)-$(VERSION).exe

all:	release

pkgs:
	pip install -r requirements.txt

continuous:
	pyinstaller --onefile microbarto.py --name $(BUILD_EXEFILE)

release:	exe
	pyinstaller --onefile microbarto.py --name $(RELEASE_EXEFILE)

run:	exe
	./dist/microbarto

lint:
	flake8 microbarto.py

format:
	black microbarto.py

clean:
	rm -r -f dist
	rm -r -f build
	rm -f *.spec

version:
	echo $(VERSION)

.PHONY:	venv exe pkgs
