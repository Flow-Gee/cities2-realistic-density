import React from 'react';
import SettingsBox from './components/_settings_box.jsx';

const HouseholdSettings = ({ model, update, trigger }) => {

    const { Grid, FormGroup, FormCheckBox, Dropdown, Icon } = window.$_gooee.framework;

    const options = [
        { label: "Default", value: "default" },
        { label: "Medium", value: "medium" },
        { label: "High", value: "high" },
        { label: "Extreme", value: "extreme" },
        { label: "Manual", value: "manual" },
    ];

    return <div>
        <Grid className="h-100">
            <div className="col p-4 bg-black-trans-faded rounded-sm">
                <div class="d-flex flex-row align-items-center mb-2">
                    <h2 className="text-primary mr-2">Household Settings</h2>
                    <p>Zone settings description</p>
                </div>
                <Grid>
                    <div class="col">
                        {!model.IsEnabled && <div className="alert alert-danger mb-2">
                            <div className="d-flex flex-row align-items-center">
                                <Icon fa className="mr-2" icon="solid-circle-exclamation"></Icon>
                                <div>Mod is globally disabled.</div>
                            </div>
                        </div>}
                        <SettingsBox title="Commercials" description="Test description" icon="coui://GameUI/Media/Game/Icons/ZoneCommercial.svg">
                            <Grid>
                                <div className="col-6">
                                    <div className="my-3">
                                        <Dropdown size="size-md" selected={"vanilla"} options={options} />
                                        <p className="text-muted fs-sm">Some text</p>
                                    </div>
                                </div>
                                <div className="col-6"></div>
                            </Grid>
                        </SettingsBox>
                        <SettingsBox title="Offices" description="Test description" icon="coui://GameUI/Media/Game/Icons/ZoneOffice.svg">
                            <Grid>
                                <div className="col-6">
                                    <div className="my-3">
                                        <Dropdown size="size-md" selected={"vanilla"} options={options} />
                                        <p className="text-muted fs-sm">Some text</p>
                                    </div>
                                </div>
                                <div className="col-6"></div>
                            </Grid>
                        </SettingsBox>
                        <SettingsBox title="Industrials" description="Test description" icon="coui://GameUI/Media/Game/Icons/ZoneIndustrial.svg">
                            <Grid>
                                <div className="col-6">
                                    <div className="my-3">
                                        <Dropdown size="size-md" selected={"vanilla"} options={options} />
                                        <p className="text-muted fs-sm">Some text</p>
                                    </div>
                                </div>
                                <div className="col-6"></div>
                            </Grid>
                        </SettingsBox>
                    </div>
                </Grid>
            </div>
        </Grid>
    </div>;
}

export default HouseholdSettings;