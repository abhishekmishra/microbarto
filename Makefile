PROGRAM_NAME=		MicroBarTo
VERSION= 			0.0.1a
DATETIME?=			$(shell date +'%y%m%d-%H%M%S')
BUILD_EXEFILE=		$(PROGRAM_NAME)-$(VERSION)-$(DATETIME).exe
RELEASE_EXEFILE=	$(PROGRAM_NAME)-$(VERSION).exe

all:	release
	echo ${DATETIME}

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

.PHONY:	venv exe pkgs