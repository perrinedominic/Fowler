$(document).ready(function () {
  // Gets the properties of an object.
  function getFilters(o) {
    var objectToInspect;
    var result = [];

    for (objectToInspect = o; objectToInspect !== null; objectToInspect = Object.getPrototypeOf(objectToInspect)) {
      result = result.concat(Object.getOwnPropertyNames(objectToInspect));
    }

    return result;
  }
});