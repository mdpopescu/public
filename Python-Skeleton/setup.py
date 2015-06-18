try:
    from setuptools import setup
except ImportError:
    from distutils.core import setup

config = {
    'description': 'My Project',
    'author': 'Marcel Popescu',
    'url': 'https://github.com/mdpopescu/public',
    'download_url': 'https://github.com/mdpopescu/public',
    'author_email': 'mdpopescu@gmail.com',
    'version': '0.1',
    'install_requires': ['nose'],
    'packages': ['NAME'],
    'scripts': [],
    'name': 'projectname'
}

setup(**config)
