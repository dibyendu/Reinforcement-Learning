FROM ghcr.io/dibyendu/reinforcement-learning-image:ubuntu-18.04-cuda-10.2


RUN rm -rf ${HOME}/repo/*
COPY . ${HOME}/repo
USER root
RUN chown -R ${NB_UID} ${HOME}/repo
USER ${NB_USER}
RUN rm -rf ${HOME}/repo/binder ${HOME}/repo/Dockerfile ${HOME}/repo/README.md
WORKDIR ${HOME}/repo
