mergeInto(LibraryManager.library, {

  FirebaseSignIn: function(emailPtr, passwordPtr, callbackObjPtr, callbackMethodPtr, requestIdPtr) {
    var email = UTF8ToString(emailPtr);
    var password = UTF8ToString(passwordPtr);
    var callbackObj = UTF8ToString(callbackObjPtr);
    var callbackMethod = UTF8ToString(callbackMethodPtr);
    var requestId = UTF8ToString(requestIdPtr);

    if (typeof firebase === 'undefined' || !firebase.auth) {
      SendMessage(callbackObj, callbackMethod, JSON.stringify({
        requestId: requestId, success: false, error: 'Firebase not initialized'
      }));
      return;
    }

    firebase.auth().signInWithEmailAndPassword(email, password)
      .then(function(cred) {
        SendMessage(callbackObj, callbackMethod, JSON.stringify({
          requestId: requestId, success: true, userId: cred.user.uid, email: cred.user.email
        }));
      })
      .catch(function(err) {
        SendMessage(callbackObj, callbackMethod, JSON.stringify({
          requestId: requestId, success: false, error: err.message
        }));
      });
  },

  FirebaseSignUp: function(emailPtr, passwordPtr, callbackObjPtr, callbackMethodPtr, requestIdPtr) {
    var email = UTF8ToString(emailPtr);
    var password = UTF8ToString(passwordPtr);
    var callbackObj = UTF8ToString(callbackObjPtr);
    var callbackMethod = UTF8ToString(callbackMethodPtr);
    var requestId = UTF8ToString(requestIdPtr);

    if (typeof firebase === 'undefined' || !firebase.auth) {
      SendMessage(callbackObj, callbackMethod, JSON.stringify({
        requestId: requestId, success: false, error: 'Firebase not initialized'
      }));
      return;
    }

    firebase.auth().createUserWithEmailAndPassword(email, password)
      .then(function(cred) {
        SendMessage(callbackObj, callbackMethod, JSON.stringify({
          requestId: requestId, success: true, userId: cred.user.uid, email: cred.user.email
        }));
      })
      .catch(function(err) {
        SendMessage(callbackObj, callbackMethod, JSON.stringify({
          requestId: requestId, success: false, error: err.message
        }));
      });
  },

  FirebaseSignOut: function() {
    if (typeof firebase !== 'undefined' && firebase.auth) {
      firebase.auth().signOut();
    }
  },

  FirebaseGetDoc: function(collectionPtr, docIdPtr, callbackObjPtr, callbackMethodPtr, requestIdPtr) {
    var collection = UTF8ToString(collectionPtr);
    var docId = UTF8ToString(docIdPtr);
    var callbackObj = UTF8ToString(callbackObjPtr);
    var callbackMethod = UTF8ToString(callbackMethodPtr);
    var requestId = UTF8ToString(requestIdPtr);

    firebase.firestore().collection(collection).doc(docId).get()
      .then(function(doc) {
        if (doc.exists) {
          SendMessage(callbackObj, callbackMethod, JSON.stringify({
            requestId: requestId, success: true, data: doc.data()
          }));
        } else {
          SendMessage(callbackObj, callbackMethod, JSON.stringify({
            requestId: requestId, success: false, error: 'Document not found'
          }));
        }
      })
      .catch(function(err) {
        SendMessage(callbackObj, callbackMethod, JSON.stringify({
          requestId: requestId, success: false, error: err.message
        }));
      });
  },

  FirebaseSetDoc: function(collectionPtr, docIdPtr, dataJsonPtr, callbackObjPtr, callbackMethodPtr, requestIdPtr) {
    var collection = UTF8ToString(collectionPtr);
    var docId = UTF8ToString(docIdPtr);
    var dataJson = UTF8ToString(dataJsonPtr);
    var callbackObj = UTF8ToString(callbackObjPtr);
    var callbackMethod = UTF8ToString(callbackMethodPtr);
    var requestId = UTF8ToString(requestIdPtr);

    var data = JSON.parse(dataJson);
    firebase.firestore().collection(collection).doc(docId).set(data, {merge: true})
      .then(function() {
        SendMessage(callbackObj, callbackMethod, JSON.stringify({
          requestId: requestId, success: true
        }));
      })
      .catch(function(err) {
        SendMessage(callbackObj, callbackMethod, JSON.stringify({
          requestId: requestId, success: false, error: err.message
        }));
      });
  },

  FirebaseQuery: function(collectionPtr, fieldPtr, valuePtr, callbackObjPtr, callbackMethodPtr, requestIdPtr) {
    var collection = UTF8ToString(collectionPtr);
    var field = UTF8ToString(fieldPtr);
    var value = UTF8ToString(valuePtr);
    var callbackObj = UTF8ToString(callbackObjPtr);
    var callbackMethod = UTF8ToString(callbackMethodPtr);
    var requestId = UTF8ToString(requestIdPtr);

    firebase.firestore().collection(collection).where(field, '==', value).get()
      .then(function(snapshot) {
        var results = [];
        snapshot.forEach(function(doc) {
          var item = doc.data();
          item.__docId__ = doc.id;
          results.push(item);
        });
        SendMessage(callbackObj, callbackMethod, JSON.stringify({
          requestId: requestId, success: true, results: results
        }));
      })
      .catch(function(err) {
        SendMessage(callbackObj, callbackMethod, JSON.stringify({
          requestId: requestId, success: false, error: err.message
        }));
      });
  }
});
