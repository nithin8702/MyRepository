# -*- coding: utf-8 -*-
"""
Created on Sat Oct  7 19:36:30 2017

@author: Srinika
"""
import pandas as pd
from sklearn import linear_model
import matplotlib.pyplot as plt

dataset = pd.read_csv('input_data.csv')
print(len(dataset))
print(dataset.columns)
print(dataset.columns.values)
a = zip(dataset['square_feet'],dataset['price'])
print(list(a))
print(dataset)
dataset['square_feet'].values
#b = list(dataset['square_feet'])

def split_data(dataset):
    s=[]
    p=[]
    for sf,pr in zip(dataset['square_feet'],dataset['price']):
        s.append([sf])
        p.append(pr)
    return s,p

#b = dataset['square_feet'].values
#c = dataset['price'].values
#b = [1,2,3,4,5]
#c = [7,6,4,3,8]

b ,c = split_data(dataset)

regr = linear_model.LinearRegression()
regr.fit(b,c)
print(regr.predict(700))