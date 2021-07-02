# From https://gitlab.com/nvidia/container-images/cuda/blob/master/doc/supported-tags.md#cuda-102
FROM nvidia/cuda:10.2-runtime-ubuntu18.04

ARG XFONT_SRC=http://security.ubuntu.com/ubuntu/pool/main/libx/libxfont/libxfont1_1.5.1-1ubuntu0.16.04.4_amd64.deb
ARG XVFB_SRC=http://security.ubuntu.com/ubuntu/pool/universe/x/xorg-server/xvfb_1.18.4-0ubuntu0.12_amd64.deb
ARG ANACONDA_SRC=https://repo.anaconda.com/miniconda/Miniconda3-py38_4.9.2-Linux-x86_64.sh




# Install dependencies
RUN apt-get -y update
RUN apt-get -y --no-install-recommends install unzip wget cmake make python-opengl g++ libz-dev xserver-common libpixman-1-0 libfontenc1 libfreetype6 xauth
RUN wget ${XFONT_SRC} && \
    wget ${XVFB_SRC} && \
    yes | dpkg -i $(echo $XFONT_SRC | sed 's/.*\///') && \
    yes | dpkg -i $(echo $XVFB_SRC | sed 's/.*\///')


# Create default user
ARG NB_USER=jovyan
ARG NB_UID=1000
ENV USER ${NB_USER}
ENV NB_UID ${NB_UID}
ENV HOME /home/${NB_USER}
RUN adduser --disabled-password --gecos "Default user" --uid ${NB_UID} ${NB_USER}


# Make sure the contents of our repo are in ${HOME}/repo
RUN mkdir -p ${HOME}/repo
COPY . ${HOME}/repo
RUN rm -rf ${HOME}/repo/binder ${HOME}/repo/Dockerfile ${HOME}/repo/README.md
RUN chown -R ${NB_UID} ${HOME}
USER ${NB_USER}
WORKDIR ${HOME}


# Install anaconda
RUN wget $ANACONDA_SRC
RUN bash $(echo $ANACONDA_SRC | sed 's/.*\///') -b
RUN rm $(echo $ANACONDA_SRC | sed 's/.*\///')
ENV PATH ${HOME}/miniconda3/bin:$PATH
# RUN conda update -n base -c defaults conda


RUN conda create -n reinforcement_learning python=3.8
SHELL ["conda", "run", "-n", "reinforcement_learning", "/bin/bash", "-c"]


RUN conda install pytorch cudatoolkit=10.2 -c pytorch
RUN conda install -c conda-forge matplotlib==3.4.1
RUN conda install -c conda-forge jupyter==1.0.0
RUN conda install -c conda-forge jupyter_contrib_nbextensions==0.5.1

RUN jupyter nbextension disable contrib_nbextensions_help_item/main && \
    jupyter nbextension disable nbextensions_configurator/config_menu/main && \
    jupyter nbextension enable freeze/main && \
    jupyter nbextension enable notify/notify && \
    jupyter nbextension enable addbefore/main && \
    jupyter nbextension enable scratchpad/main && \
    jupyter nbextension enable scroll_down/main && \
    jupyter nbextension enable codefolding/main && \
    jupyter nbextension enable spellchecker/main && \
    jupyter nbextension enable splitcell/splitcell && \
    jupyter nbextension enable execute_time/ExecuteTime && \
    jupyter nbextension enable toggle_all_line_numbers/main

RUN conda install -c conda-forge pybox2d==2.3.10
RUN conda install -c conda-forge seaborn==0.11.1
RUN pip install PyVirtualDisplay==2.1
RUN pip install mlagents-envs==0.25.1
RUN pip install gym==0.18.0
RUN pip install gym[atari]


WORKDIR ${HOME}/repo
ENTRYPOINT ["conda", "run", "--no-capture-output", "-n", "reinforcement_learning"]
