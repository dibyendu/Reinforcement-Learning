[![Binder](https://mybinder.org/badge_logo.svg)](https://mybinder.org/v2/gh/dibyendu/Reinforcement-Learning/main)

## Standalone installation on local machine using python virtual environemnt

#### List the existing environment(s)
```
conda env list
```

#### Create a new conda environment
```
conda create -n rl python=3.8
```

#### Activate the environment
```
conda activate rl
```

#### Install the required packages
```
conda install pytorch cudatoolkit=10.2 -c pytorch
conda install -c conda-forge matplotlib==3.4.1
conda install -c conda-forge jupyter==1.0.0
conda install -c conda-forge jupyter_contrib_nbextensions==0.5.1
conda install -c conda-forge pybox2d==2.3.10
conda install -c conda-forge seaborn==0.11.1
pip install PyVirtualDisplay==2.1
pip install mlagents-envs==0.25.1
pip install gym==0.18.0
pip install gym[atari]
```

#### Deactivate the environment
```
conda deactivate
```

#### (Optional) Export the environment
```
conda env export --no-builds -f environment.yml
```

#### (Optional) Delete the environment
```
conda remove --name rl --all
```

## Docker installation

#### Build the container
```
docker build --tag image_name:image_tag github.com/dibyendu/Reinforcement-Learning
```

#### Run the container
```
docker run --publish 127.0.0.1:8080:5678 --publish 127.0.0.1:9999:9999 image_name:image_tag jupyter notebook --ip 0.0.0.0 --port 5678 --NotebookApp.custom_display_url=http://localhost:8080
```
#### (Optional) Delete the container
```
docker image rm --force image_name:image_tag
```
