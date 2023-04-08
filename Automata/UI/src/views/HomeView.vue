<template>
  <div>
    <div class="d-flex align-center justify-end gap-1rem pa-4">
      <v-btn color="primary" fab :to="{ name: 'info' }">
        <v-icon>mdi-help</v-icon>
      </v-btn>
      <v-speed-dial v-model="languageSwitcherOpened" direction="bottom">
        <template v-slot:activator>
          <v-btn
            v-model="languageSwitcherOpened"
            fab
            dark
            class="language-btn"
            v-html="selectedLanguage.svg"
          >
          </v-btn>
        </template>
        <v-btn
          fab
          dark
          class="language-btn"
          v-for="language in languages"
          :key="language.key"
          v-html="language.svg"
          @click="SetLocale(language)"
        >
        </v-btn>
      </v-speed-dial>
    </div>
    <v-container class="home">
      <v-row>
        <v-col cols="12">
          <div class="text-h3 text-center primary--text font-weight-medium">
            {{ $t('homeTitle') }}
          </div>
        </v-col>
      </v-row>
      <v-row>
        <v-col
          class="menu-item"
          cols="6"
          md="6"
          v-for="menuItem in menuItems"
          :key="menuItem.key"
          ref="menuItems"
        >
          <v-card class="fill-height" :to="menuItem.to">
            <v-img
              :src="menuItem.img"
              contain
              :max-height="menuItemImageHeight"
            >
            </v-img>
            <v-card-title class="text-h5 text-md-h3 pb-5">
              {{ menuItem.title }}
            </v-card-title>

            <v-card-text>
              {{ menuItem.description }}
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<script>
import { LanguageList } from '@/data/languages';
import { AppFunctions } from '@/functions/AppFunctions';
import router from '@/router';
import { useCountdown } from '@/utils/useCountdown';
import { gsap, Linear } from 'gsap';
export default {
  name: 'Home',
  data() {
    return {
      languageSwitcherOpened: false,
    };
  },
  setup() {
    useCountdown(20 * 1000, () => {
      router.push({ name: 'welcome' });
    });
  },
  mounted() {
    gsap.from(this.$refs.menuItems, {
      duration: 0.3,
      translateX: -100,
      opacity: 0,
      ease: 'power1.inOut',
      stagger: {
        each: 0.2,
      },
    });
  },

  methods: {
    SetLocale(language) {
      this.$i18n.locale = language.key;
    },
  },
  computed: {
    menuItems() {
      return [
        {
          id: 1,
          title: this.$t('menuItems.carPickUp.title'),
          img: require('@/assets/illustrations/car-pick-up.jpg'),
          description: this.$t('menuItems.carPickUp.description'),
          to: {
            name: 'car-pick-up',
          },
        },
        {
          id: 2,
          title: this.$t('menuItems.carDropOff.title'),
          img: require('@/assets/illustrations/car-drop-off.jpg'),
          description: this.$t('menuItems.carDropOff.description'),
          to: {
            name: 'car-drop-off',
          },
        },
      ];
    },
    languages() {
      let selectedLanguage = this.selectedLanguage;
      return LanguageList.filter((f) => f.key != selectedLanguage.key);
    },
    selectedLanguage() {
      return AppFunctions.GetLanguage(this.$i18n.locale);
    },
    menuItemImageHeight() {
      return this.$vuetify.breakpoint.mdAndDown ? '300px' : '500px';
    },
  },
};
</script>
<style lang="scss" scoped>
.home {
  background-color: var(--yellow-light);
}
.language-btn {
  overflow: hidden;
  svg {
    height: 40px;
    width: 40px;
  }
}
.v-card {
  .v-card__title {
    justify-content: center;
  }
  .v-card__text {
    text-align: center;
  }
}
</style>
