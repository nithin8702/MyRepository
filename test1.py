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