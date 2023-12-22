import React from 'react';
import { useDataUpdate } from 'hookui-framework';
import $TabWindow from './components/_tab-window';
import $Settings from './tabs/_settings';
import $About from './tabs/_about';
import $CityServices from './tabs/_city-services';
import $Spawnables from './tabs/_spawnables';

const $RealisticDensity = ({ react }) => {

    const [data, setData] = react.useState({})
    useDataUpdate(react, "89pleasure_realisticdensity.config", setData)

    //defaultThemes
    const triggerUpdate = (prop, val) => {
        engine.trigger("89pleasure_realisticdensity.updateProperty", JSON.stringify({ property: prop, value: val }));
    };

    const toggleVisibility = () => {
        const visData = { type: "toggle_visibility", id: "89pleasure.realisticdensity" };
        const event = new CustomEvent('hookui', { detail: visData });
        window.dispatchEvent(event);

        engine.trigger("audio.playSound", "close-panel", 1);
    }

    const tabs = [
        {
            name: 'Info',
            content: <div style={{ height: '100%', width: '100%' }}>
            </div>
        },
        {
            name: 'City Services',
            content: <div style={{ display: 'flex', width: '100%' }}>
                <$CityServices react={react} data={data} setData={setData} triggerUpdate={triggerUpdate} />
            </div>
        },
        {
            name: 'Spawnables',
            content: <div style={{ height: '100%', width: '100%' }}>
                <$Spawnables react={react} data={data} setData={setData} triggerUpdate={triggerUpdate} />
            </div>
        },
        {
            name: 'About',
            content: <div style={{ height: '100%', width: '100%' }}>
                <$About />
            </div>
        }
    ];

    return <$TabWindow react={react} tabs={tabs} onClose={toggleVisibility} style={{ opacity: '1' }} />
};

// Registering the panel with HookUI
window._$hookui.registerPanel({
    id: "89pleasure.realisticdensity",
    name: "Realistic Density",
    icon: "Media/Game/Icons/GenericVehicle.svg",
    component: $RealisticDensity
});