<script setup lang="ts">
  import router from '@/router';
  import { useRoute } from "vue-router";
  import { watch } from "vue";
  import { ref } from 'vue'
  import type { Ref } from 'vue'
  import type { Bookmark } from "@/models";

  let loading : Ref<boolean> = ref(false);
  let saving : Ref<boolean> = ref(false);
  let error : Ref<string | null> = ref(null);
  let bookmark : Ref<Bookmark | null> = ref(null);

  const route = useRoute();

  // TODO: Tags
  // TODO: Autocomplete on tag names?
  // TODO: Create component for add/edit bookmark?

  watch(
      () => route.params,
      async () => {
        await fetchData();
      },
      { immediate: true }
  );

  async function fetchData() {
    bookmark.value = null;
    loading.value = true;

    try {
      const response = await fetch('/api/bookmark/' + route.params.id);
      loading.value = false;
      if (response.ok) {
        bookmark.value = await response.json();
      } else {
        error.value = "API responded with " + response.statusText;
      }
    } catch (err : any) {
      loading.value = false;
      error.value = err.toString();
    }
  }

  function cancel() {
    router.push('/');
  }

  async function save() {
    saving.value = true;

    try {
      const response = await fetch('/api/bookmark/', {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(bookmark.value)
      });
      if (response.ok) {
        await router.push('/');
      } else {
        saving.value = false;
        error.value = "API responded with " + response.statusText;
      }
    } catch (err : any) {
      error.value = err.toString();
    }
  }
</script>

<template>
  <h1>Edit bookmark</h1>

  <div v-if="loading">Loading...</div>
  <div v-if="error">
    <div class="alert alert-danger" role="alert">
      Bookmark not found or couldn't fetch bookmark info.

      <p class="mt-1">Errormessage: {{ error }}</p>
    </div>
  </div>
  <div v-if="bookmark">
    <div class="mb-3">
      <label for="url" class="form-label">URL</label>
      <input type="text" class="form-control" id="url" v-model="bookmark.url">
    </div>
    <div class="mb-3">
      <label for="tags" class="form-label">Tags</label>
      <input type="text" class="form-control" id="tags" v-model="bookmark.tags">
      <div class="form-text">Enter tags separated by space.</div>
    </div>
    <div class="mb-3">
      <label for="title" class="form-label">Title</label>
      <input type="text" class="form-control" id="title" v-model="bookmark.title">
      <div class="form-text">Optional, load from website.</div>
    </div>
    <div class="mb-3">
      <label for="description" class="form-label">Description</label>
      <input type="text" class="form-control" id="description" v-model="bookmark.description">
      <div class="form-text">Optional, load from website.</div>
    </div>

    <button type="button" class="btn btn-success" @click="save" v-bind:disabled="saving">
      <span v-if="saving" class="spinner-border spinner-border-sm" aria-hidden="true"></span>
      <span v-if="saving" role="status">Saving...</span>
      <span v-if="!saving">Save</span>
    </button>
    <button type="button" class="btn btn-secondary ms-2" @click="cancel">Cancel</button>
  </div>
</template>
