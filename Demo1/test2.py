# -*- coding: utf-8 -*-
"""
Created on Sat Oct 21 18:09:33 2017

@author: Srinika
"""

import pandas as pd
import numpy as np
from sklearn import linear_model
import matplotlib.pyplot as plt

dataset = pd.read_csv('tv_show_viewers_datasest.csv')
print(len(dataset))
print(dataset.columns.values)
dataset.describe()

def split_data(dataset):
    features = []
    target = []
    for ch1, ch2, ch3, ch4, ch5,fight_scene, comedy_scene,r_scene, viewers_count in zip(
                dataset['Character1_appeared'],
                dataset['Character2_appeared'],
                dataset['Character3_appeared'],
                dataset['Character4_appeared'],
                dataset['Character5_appeared'],
                dataset['Fight_scenes'],
                dataset['Comedy_scences'],
                dataset['Romance_scence'],
                dataset['Viewers'],
            ):
        features.append([ch1, ch2, ch3, ch4, ch5,fight_scene, comedy_scene,r_scene])
        target.append(viewers_count)
#    print(type(features))
#    print(type(target))
    return features,target

train_features, train_targets = split_data(dataset)

#type(train_features)
#type(train_targets)
#type(split_data)

regr = linear_model.LinearRegression()
regr.fit(train_features,train_targets)


episode51_features = np.array([4,6,3,6,3,4,8,9]).reshape(1,-1)
print(regr.predict(episode51_features))


plt.scatter(train_features,train_targets,color='blue')
plt.plot(train_features,regr.predict(train_features),color='red',linewidth=4)
plt.show()