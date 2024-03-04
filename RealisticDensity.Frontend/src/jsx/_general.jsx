import React from 'react';

const GeneralSettings = ({ model, update, trigger }) => {

    const { Grid, FormGroup, FormCheckBox } = window.$_gooee.framework;

    return <div>
        <Grid className="h-100">
            <div className="col-3 p-4 bg-black-trans-faded rounded-sm">
                <div className="bg-black-trans-less-faded rounded-sm mb-4">
                    <div class="p-4">
                        <FormGroup label="Enable Mod">
                            <small className="text-muted mb-2">Some description</small>
                            <FormCheckBox checked={model.IsEnabled} label="On/Off" onToggle={value => update("IsEnabled", value)} />
                        </FormGroup>
                    </div>
                </div>
            </div>
            <div className="col-3 p-4 bg-black-trans-faded rounded-sm">
                <div className="bg-black-trans-less-faded rounded-sm mb-4">
                    <div class="p-4">
                        <FormGroup label="Enable Mod">
                            <small className="text-muted mb-2">Some description</small>
                            <FormCheckBox checked={model.IsEnabled} label="On/Off" onToggle={value => update("IsEnabled", value)} />
                        </FormGroup>
                    </div>
                </div>
            </div>
            <div className="col-3 p-4 bg-black-trans-faded rounded-sm">
                <div className="bg-black-trans-less-faded rounded-sm mb-4">
                    <div class="p-4">
                        <FormGroup label="Enable Mod">
                            <small className="text-muted mb-2">Some description</small>
                            <FormCheckBox checked={model.IsEnabled} label="On/Off" onToggle={value => update("IsEnabled", value)} />
                        </FormGroup>
                    </div>
                </div>
            </div>
            <div className="col-3 p-4 bg-black-trans-faded rounded-sm">
                <div className="bg-black-trans-less-faded rounded-sm mb-4">
                    <div class="p-4">
                        <FormGroup label="Enable Mod">
                            <small className="text-muted mb-2">Some description</small>
                            <FormCheckBox checked={model.IsEnabled} label="On/Off" onToggle={value => update("IsEnabled", value)} />
                        </FormGroup>
                    </div>
                </div>
            </div>
        </Grid>
    </div>;
}

export default GeneralSettings;