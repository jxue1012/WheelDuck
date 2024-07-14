mergeInto(LibraryManager.library, {
  GetJSON: function (key, objectName, callback, fallback) {
    var pKey = UTF8ToString(key);
    var parsedObjectName = UTF8ToString(objectName);
    var parsedCallback = UTF8ToString(callback);
    var parsedFallback = UTF8ToString(fallback);
    GetJSON(pKey, parsedObjectName, parsedCallback, parsedFallback);
  },

  SetJSON: function (key, value, objectName, callback, fallback) {
    var pKey = UTF8ToString(key);
    var pValue = UTF8ToString(value);
    var parsedObjectName = UTF8ToString(objectName);
    var parsedCallback = UTF8ToString(callback);
    var parsedFallback = UTF8ToString(fallback);

    SetJSON(pKey, pValue, parsedObjectName, parsedCallback, parsedFallback);
  },
});
