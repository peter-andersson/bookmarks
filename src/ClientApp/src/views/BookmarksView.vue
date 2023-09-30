<script setup lang="ts">
  import { watch } from "vue";
  import { useRoute } from "vue-router";
  import { ref } from 'vue'

  const route = useRoute();

  let loading = ref(false);
  let error = ref(null);
  let bookmarks = ref(null);

  watch(
      () => route.fullPath,
      async () => {
        await fetchData();
      },
      { immediate: true }
  );

  async function fetchData() {
    bookmarks.value = null;
    error.value = null;
    loading.value = true;

    try {
      const response = await fetch('/api/bookmark');
      bookmarks.value = await response.json();
      loading.value = false;
    } catch (err) {
      loading.value = false;
      error.value = err.toString();
    }
  }
</script>

<template>
  <div class="row">
    <div class="input-group mb-3">
      <input type="text" class="form-control" placeholder="Search for bookmarks">
      <span class="input-group-text" id="basic-addon1"><button class="btn btn-sm btn-primary" type="button">Search</button></span>
    </div>
  </div>
  <div class="row">
    <div v-if="loading">Loading...</div>
    <div v-if="error">{{ error }}</div>
    <div v-if="bookmarks">TODO: Show bookmarks</div>
  </div>
</template>
