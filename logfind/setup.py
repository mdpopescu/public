try:
    from setuptools import setup
except ImportError:
    from distutils.core import setup

config = {
    'description': 'logfind',
    'author': 'Marcel Popescu',
    'url': 'https://github.com/mdpopescu/public',
    'download_url': 'https://github.com/mdpopescu/public',
    'author_email': 'mdpopescu@gmail.com',
    'version': '0.1',
    'install_requires': ['nose'],
    'packages': ['logfind'],
    'scripts': [],
    'name': 'logfind'
}

setup(**config)
