import React from "react";
import About from "./_about.jsx";
import "./styles.jsx";
import GeneralSettings from "./_general.jsx";
import HouseholdSettings from "./_household.jsx";
import WorkforceSettings from "./_workforce.jsx";

const RealisticDensityButton = ({ react, setupController }) => {
    const onMouseEnter = () => {
        engine.trigger("audio.playSound", "hover-item", 1);
    };

    const { AutoToolTip, ToolTipContent } = window.$_gooee.framework;

    const { model, trigger, _L } = setupController();

    const onClick = () => {
        const newValue = !model.IsVisible;
        trigger("OnToggleVisible");
        engine.trigger("audio.playSound", "select-item", 1);

        if (newValue) {
            engine.trigger("audio.playSound", "open-panel", 1);
            engine.trigger("tool.selectTool", null);
        }
        else
            engine.trigger("audio.playSound", "close-panel", 1);
    };

    const description = `Open the Realistic Density v${model.Version} mod panel.`;
    const realisticDensityBtnRef = react.useRef(null);

    return model.IsEnabled ? <>
        <div className="spacer_oEi"></div>
        <button onMouseEnter={onMouseEnter} onClick={onClick} class="button_s2g button_ECf item_It6 item-mouse-states_Fmi item-selected_tAM item-focused_FuT button_s2g button_ECf item_It6 item-mouse-states_Fmi item-selected_tAM item-focused_FuT toggle-states_X82 toggle-states_DTm">
            <div className="fa fa-solid-magnifying-glass icon-md"></div>
            <AutoToolTip targetRef={realisticDensityBtnRef} float="up" align="right">
                <ToolTipContent title="Realistic Density" description={description} />
            </AutoToolTip>
        </button>
    </> : null;
};
window.$_gooee.register("realisticdensity", "RealisticDensityIconButton", RealisticDensityButton, "bottom-right-toolbar", "realisticdensity");

const RealisticDensityContainer = ({ react, setupController }) => {
    const { TabModal } = window.$_gooee.framework;
    const { model, update, trigger, _L } = setupController();

    const tabs = [
        {
            name: "WORKFORCE",
            label: <div>Workforce</div>,
            content: <WorkforceSettings react={react} model={model} update={update} trigger={trigger} _L={_L} />
        },
        {
            name: "GENERAL",
            label: <div>General</div>,
            content: <GeneralSettings model={model} update={update} trigger={trigger} _L={_L} />
        },
        {
            name: "HOUSEHOLD",
            label: <div>Households</div>,
            content: <HouseholdSettings model={model} update={update} trigger={trigger} _L={_L} />
        },
        {
            name: "ABOUT",
            label: <div>About</div>,
            content: <About react={react} model={model} update={update} _L={_L} />
        }
    ];

    const closeModal = () => {
        trigger("OnToggleVisible");
        engine.trigger("audio.playSound", "close-panel", 1);
    };

    return model.IsVisible ? <TabModal fixed size="xl" title="Realistic Density" tabs={tabs} onClose={closeModal} /> : null;
};
window.$_gooee.register("realisticdensity", "RealisticDensityContainer", RealisticDensityContainer, "main-container", "realisticdensity");