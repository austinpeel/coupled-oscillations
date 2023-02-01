import React, { useCallback, useEffect } from 'react';
import { Unity, useUnityContext } from 'react-unity-webgl';
import './UnityApp.css';

function UnityApp(props) {
  const {
    unityProvider,
    isLoaded,
    loadingProgression,
    requestFullscreen,
    addEventListener,
    removeEventListener,
  } = useUnityContext({
    loaderUrl: props.loaderUrl,
    dataUrl: props.dataUrl,
    frameworkUrl: props.frameworkUrl,
    codeUrl: props.codeUrl,
  });

  const loadingPercentage = Math.round(loadingProgression * 100);

  const handleEnterFullScreen = useCallback(() => {
    requestFullscreen(true);
  }, [requestFullscreen]);

  useEffect(() => {
    addEventListener('EnterFullscreen', handleEnterFullScreen);
    return () => {
      removeEventListener('EnterFullscreen', handleEnterFullScreen);
    };
  }, [addEventListener, removeEventListener, handleEnterFullScreen]);

  const handleExitFullScreen = useCallback(() => {
    requestFullscreen(false);
  }, [requestFullscreen]);

  useEffect(() => {
    addEventListener('ExitFullscreen', handleExitFullScreen);
    return () => {
      removeEventListener('ExitFullscreen', handleExitFullScreen);
    };
  }, [addEventListener, removeEventListener, handleExitFullScreen]);

  return (
    <div id='unity-app' className='unity-app'>
      {!isLoaded && (
        <div className='loading-overlay'>
          <p>Loading... ({loadingPercentage}%)</p>
        </div>
      )}
      <Unity unityProvider={unityProvider} className='unity' />
      {/* <button onClick={handleClickEnterFullscreen}>Full screen</button> */}
    </div>
  );
}

export default UnityApp;
