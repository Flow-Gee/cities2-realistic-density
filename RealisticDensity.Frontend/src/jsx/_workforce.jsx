import React from 'react';
import SettingsBox from './components/_settings_box.jsx';

const WorkforceSettings = ({ react, model, update, trigger }) => {

    const { Grid, Button, Dropdown, Icon } = window.$_gooee.framework;

    const options = [
        { label: "Default", value: "default" },
        { label: "Medium", value: "medium" },
        { label: "High", value: "high" },
        { label: "Extreme", value: "extreme" },
        { label: "Manual", value: "manual" },
    ];

    const [category, setCategory] = react.useState("spawnables");
    const onCategoryChanged = (value) => {
        setCategory(value);
    };

    const menuButton = (id, title, icon) => {
        return <Button size="sm" style="trans" color={category == id ? 'dark' : null} onClick={() => onCategoryChanged(id)}>
            <div className="d-flex flex-row align-items-center">
                <Icon className="mr-1" size="sm" icon={icon} />
                <p>{title}</p>
            </div>
        </Button>
    };

    const categoryContent = [
        {
            name: "spawnables",
            content: <div>
                <Grid>
                    <SettingsBox title="Zoneables" icon="coui://GameUI/Media/Game/Icons/Zones.svg">
                        <p>Test description</p>
                    </SettingsBox>
                </Grid>
                <Grid>
                    <SettingsBox title="Commercial" description="Test description" icon="coui://GameUI/Media/Game/Icons/ZoneCommercial.svg">
                        <div className="my-3">
                            <Dropdown size="size-md" selected={"default"} options={options} />
                            <p className="text-muted fs-sm">Some text</p>
                        </div>
                    </SettingsBox>
                </Grid>
                <Grid>
                    <SettingsBox title="Commercial" description="Test description" icon="coui://GameUI/Media/Game/Icons/ZoneCommercial.svg">
                        <div className="my-3">
                            <Dropdown size="size-md" selected={"default"} options={options} />
                            <p className="text-muted fs-sm">Some text</p>
                        </div>
                    </SettingsBox>
                </Grid>
            </div>
        },
        {
            name: "healthcare",
            content: <Grid>
                <div className="col-3">
                    <SettingsBox title="Schools" description="Test description" icon="coui://GameUI/Media/Game/Icons/ZoneIndustrial.svg">
                        <Grid>
                            <div className="col-6">
                                <div className="my-3">
                                    <Dropdown size="size-md" selected={"default"} options={options} />
                                    <p className="text-muted fs-sm">Some text</p>
                                </div>
                            </div>
                            <div className="col-6"></div>
                        </Grid>
                    </SettingsBox>
                </div>
                <div className="col-3">
                    <SettingsBox title="Hospitals & Clinics" description="Test description" icon="coui://GameUI/Media/Game/Icons/ZoneIndustrial.svg">
                        <Grid>
                            <div className="col-6">
                                <div className="my-3">
                                    <Dropdown size="size-md" selected={"default"} options={options} />
                                    <p className="text-muted fs-sm">Some text</p>
                                </div>
                            </div>
                            <div className="col-6"></div>
                        </Grid>
                    </SettingsBox>
                </div>
                <div className="col-3">
                    <SettingsBox title="Police" description="Test description" icon="coui://GameUI/Media/Game/Icons/ZoneIndustrial.svg">
                        <Grid>
                            <div className="col-6">
                                <div className="my-3">
                                    <Dropdown size="size-md" selected={"default"} options={options} />
                                    <p className="text-muted fs-sm">Some text</p>
                                </div>
                            </div>
                            <div className="col-6"></div>
                        </Grid>
                    </SettingsBox>
                </div>
                <div className="col-3">
                    <SettingsBox title="Fire brigade" description="Test description" icon="coui://GameUI/Media/Game/Icons/ZoneIndustrial.svg">
                        <Grid>
                            <div className="col-6">
                                <div className="my-3">
                                    <Dropdown size="size-md" selected={"default"} options={options} />
                                    <p className="text-muted fs-sm">Some text</p>
                                </div>
                            </div>
                            <div className="col-6"></div>
                        </Grid>
                    </SettingsBox>
                </div>
            </Grid>
        },
    ];

    return <div className="p-4 bg-black-trans-faded rounded-sm">
        <Grid className="h-100">
            <div className="col">
                <div class="d-flex flex-column align-items-start mb-4">
                    <h2 className="text-primary mr-2">Workforce Settings</h2>
                    <Grid>
                        <div className="col-6">
                            <p>Adjust workforce numbers to increase the available workplaces. Higher numbers will lead to higher density in your city.</p>
                        </div>
                        <div className="col-6">
                            
                        </div>
                    </Grid>
                    
                </div>
                <Grid>
                    <div className="col-3 bg-black-trans-less-faded rounded-sm">
                        <div className="btn-group-vertical w-100">
                            {menuButton("spawnables", "Spawnables", "coui://GameUI/Media/Game/Icons/Information.svg")}
                            {menuButton("healthcare", "Healthcare", "coui://GameUI/Media/Game/Icons/Healthcare.svg")}
                        </div>
                    </div>
                    <div className="col-9">
                        {categoryContent.find(x => x['name'] === category).content}
                    </div>
                </Grid>
            </div>
        </Grid>
    </div>;
}

export default WorkforceSettings;