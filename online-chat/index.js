// например, в index.js
const setBodyHeight = () => {
  if (window.innerWidth < 1050) {
    document.body.style.height = window.innerHeight + 'px';
  } else {
    document.body.style.height = '100%';
  }
};

window.addEventListener('resize', setBodyHeight);
setBodyHeight();
