package com.e.augmentedimages;

import com.google.ar.core.Config;
import com.google.ar.core.Session;
import com.google.ar.sceneform.ux.ArFragment;

public class CustomArFrag extends ArFragment {
    @Override
    protected Config getSessionConfiguration(Session session) {
        Config config = new Config(session);
        config.setUpdateMode(Config.UpdateMode.LATEST_CAMERA_IMAGE);
        session.configure(config);

        this.getArSceneView().setupSession(session);

        ((MainActivity)getActivity()).setupDatabase(config,session);
        return config;
    }
}
