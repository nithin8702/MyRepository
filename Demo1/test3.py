# -*- coding: utf-8 -*-
"""
Created on Sun Oct 22 20:03:53 2017

@author: Srinika
"""

import pandas as pd
from sklearn.linear_model import LogisticRegression
from sklearn.model_selection import train_test_split
from sklearn import metrics

anes_dataset = pd.read_csv('anes_dataset.csv')

print(anes_dataset.columns.values)
print(anes_dataset.shape[0]) #row count
print(anes_dataset.shape[1]) #column count

headers = list(anes_dataset.columns.values)
features = headers[:-1]
target = headers[-1]

x_train,x_test,y_train,y_test= train_test_split(anes_dataset[features],anes_dataset[target],test_size=0.4)

features_4 = ['TVnews', 'age', 'educ', 'income']
model_with_features_4 = LogisticRegression()
model_with_features_4.fit(x_train[features_4],y_train)
train_accuracy_4 = model_with_features_4.score(x_train[features_4],y_train)
print('train_accuracy:',train_accuracy_4)

model_with_all_features = LogisticRegression()
model_with_all_features.fit(x_train,y_train)
train_accuracy = model_with_all_features.score(x_train,y_train)
print('train_accuracy:',train_accuracy)

#testing with first row for prediction
test_observ_4 = x_test[features_4][:1]
test_observ = x_test[features][:1]
print(model_with_features_4.predict(test_observ_4))
print(model_with_all_features.predict(test_observ))

model_with_features_4_prediction = model_with_features_4.predict(x_test[features_4])
model_with_features_4_test_accuracy = metrics.accuracy_score(y_test,model_with_features_4_prediction)
model_with_features_all_prediction = model_with_all_features.predict(x_test)
model_with_features_all_test_accuracy = metrics.accuracy_score(y_test,model_with_features_all_prediction)

print(model_with_features_4_prediction)
print(model_with_features_4_test_accuracy)
print(model_with_features_all_prediction)
print(model_with_features_all_test_accuracy)



